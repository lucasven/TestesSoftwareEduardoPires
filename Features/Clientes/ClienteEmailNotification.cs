using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Features.Clientes
{
    public class ClienteEmailNotification : INotification
    {
        public string Origem { get; set; }
        public string Destino { get; set; }
        public string Assunto { get; set; }
        public string Mensagem { get; set; }

        public ClienteEmailNotification(string origem, string destino, string assunto, string mensagem)
        {
            Origem = origem;
            Destino = destino;
            Assunto = assunto;
            Mensagem = mensagem;
        }
    }
}
