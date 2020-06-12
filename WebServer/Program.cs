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

        // Запуск сервера
        public Server(int Port)
        {
            // Создаем "слушателя" для указанного порта
            Listener = new TcpListener(IPAddress.Any, Port); // IPAddress.Any - сервер прослушивает подключения по всем интерфейсам
            Listener.Start(); // Запуск сервера

            // В бесконечном цикле
            while (true)
            {
                // Принимаем новых клиентов
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
            // Создадим новый сервер на порту 80
            new Server(5980);
        }
    }

    class Client
    {
        // Конструктор класса. Ему нужно передавать принятого клиента от TcpListener
        public Client(TcpClient Client)
        {
            // Код простой HTML-странички
            string Html = "<html><body><h1>It works!</h1></body></html>";
            // Необходимые заголовки: ответ сервера, тип и длина содержимого. После двух пустых строк - само содержимое
            string Str = "HTTP/1.1 200 OK\nContent-type: text/html\nContent-Length:" + Html.Length.ToString() + "\n\n" + Html;
            // Приведем строку к виду массива байт
            byte[] Buffer = Encoding.ASCII.GetBytes(Str);
            // Отправим его клиенту

            Client.GetStream().Write(Buffer, 0, Buffer.Length);
            // Отправим его клиенту

            Console.WriteLine("Connect client");

            // Закроем соединение

            Client.Close();

        }
    }
}
