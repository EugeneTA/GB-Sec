using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patterns
{
    internal class Builder
    {
        static void Main(string[] args)
        {
            MessageBuilder builder = new MessageBuilder();
            try
            {
                Console.WriteLine(builder.From("test@mail.com").To("user@mail.com").Subject("Test message").Body("Hello world!").Build());
                Console.WriteLine(builder.From("").To("user@mail.com").Subject("Test message").Body("Hello world!").Build());
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }

    public class Message
    {
        public string? From { get; set; }
        public string? To { get; set; }
        public string? Subject { get; set; }
        public string? Body { get; set; }

        public override string ToString()
        {
            return $"Message:{Environment.NewLine}{From}{Environment.NewLine}{To}{Environment.NewLine}{Subject}{Environment.NewLine}{Body}{Environment.NewLine}";
        }

    }

    public class MessageBuilder
    {
        private readonly Message _message = new Message();
        public Message Build()
        {
            if (string.IsNullOrEmpty(_message.From) && string.IsNullOrEmpty(_message.From))
            {
                throw new InvalidOperationException("Fields From or To are empty.");
            };
            return _message;
        }

        public MessageBuilder From(string data)
        {
            _message.From = data;
            return this;
        }
        public MessageBuilder To(string data)
        {
            _message.To = data;
            return this;
        }
        public MessageBuilder Subject(string data)
        {
            _message.Subject = data;
            return this;
        }
        public MessageBuilder Body(string data)
        {
            _message.Body = data;
            return this;
        }
    }
}
