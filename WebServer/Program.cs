using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;


namespace WebServer
{
    class Server
    {
        TcpListener Listener; // Объект, прослушивающий подключения по TCP

        // Web-сервер
        public Server(int Port)
        {
            // Создаем "слушателя" для указанного порта
            // IPAddress.Any - сервер прослушивает подключения по всем интерфейсам
            Listener = new TcpListener(IPAddress.Any, Port);
            // Метод Start начинает помещать все запросы от клиентов в очередь, пока не будет вызван метод stop 
            Listener.Start(); 
            // Принимаем новых клиентов в бесконечном цикле
            while (true)
            {
                // Создание экземпляра класса Client, описанного ниже. 
                // Listener.AcceptTcpClient() - блокирует выполнение программы, пока не подключится клиент
                // После подключения клиента возвращает экземпляр класса TcpClient
                new Client(Listener.AcceptTcpClient());
            }
        }

        // Остановка сервера
        ~Server()
        {
            // Если "слушатель" был создан
            if (Listener != null)
            {
                // Остановим его
                Listener.Stop();
            }
        }

        static void Main(string[] args)
        {
            // Создадим новый сервер, прослушивающий порту 80
            new Server(80);
        }
    }

    class Client
    {
        // Конструктор класса. Ему нужно передавать принятого клиента от TcpListener
        public Client(TcpClient Client)
        {
            // Вывод сообщения о подключении клиента
            Console.WriteLine("Client connected\n");
            byte[] Buffer = new Byte[1024]; // Буфер для полученного сообщения
            Int32 readBytesCount; // Число полученных байт
            String RequestData = ""; // Строка с данными запроса
            // Метод Read возвращает число прочитанных байт, пока он возвращает  больше 0 выполняем цикл
            readBytesCount = Client.GetStream().Read(Buffer, 0, Buffer.Length);
            RequestData = RequestData + System.Text.Encoding.ASCII.GetString(Buffer, 0, readBytesCount);
            Console.WriteLine("Received: \n {0}", RequestData);


            // Сформируем строку с HTML сообщением для отправки клиенту
            // Стартовая строка HTML сообщения
            String responseData = "HTTP/1.1 200 OK\n";
            // Заголовки HTML сообщения
            // Присвоим строке содержимое самой простой web-страницы
            string Html = "<html><body><h1>It works!</h1></body></html>";
            // Добавим в строку заголовок с типом контента
            responseData = responseData + "Content-type: text/html\n";
            // Добавим в строку заголовок с размером контента
            responseData = responseData + "Content-Length:" + Html.Length.ToString() + "\n\n";
            // Тело HTML сообщения
            responseData = responseData + Html;

            // Приведем строку ответа к виду массива байт
            Buffer = Encoding.ASCII.GetBytes(responseData);
            // Отправим её клиенту
            Client.GetStream().Write(Buffer, 0, Buffer.Length);
            Console.WriteLine("Send: \n {0}", responseData);
            // Пропуск двух строк
            Console.WriteLine("\n\n");
            // Закроем соединение
            Client.Close();

        }
    }
}
