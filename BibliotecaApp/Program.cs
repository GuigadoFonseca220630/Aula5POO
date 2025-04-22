using System;
using System.Collections.Generic;
using System.Linq;

namespace BibliotecaApp
{
    // Interface de Notificação
    public interface INotificacaoService
    {
        void EnviarNotificacao(string destinatario, string assunto, string mensagem);
    }

    // Implementação do Serviço de Notificação
    public class NotificacaoService : INotificacaoService
    {
        public void EnviarNotificacao(string destinatario, string assunto, string mensagem)
        {
            Console.WriteLine($"[NOTIFICAÇÃO] Para: {destinatario}\nAssunto: {assunto}\nMensagem: {mensagem}");
        }
    }

    // Classe de Gerenciamento de Livros
    public class GerenciadorLivros
    {
        private readonly List<Livro> livros = new List<Livro>();

        public void AdicionarLivro(string titulo, string autor, string isbn)
        {
            if (livros.Any(l => l.ISBN == isbn))
            {
                throw new Exception("Erro: ISBN já cadastrado.");
            }
            livros.Add(new Livro { Titulo = titulo, Autor = autor, ISBN = isbn, Disponivel = true });
            Console.WriteLine($"Livro '{titulo}' cadastrado com sucesso!");
        }

        public Livro BuscarLivro(string isbn)
        {
            return livros.FirstOrDefault(l => l.ISBN == isbn && l.Disponivel)
                   ?? throw new Exception("Livro não disponível ou não encontrado.");
        }

        public void AtualizarDisponibilidade(string isbn, bool disponivel)
        {
            var livro = livros.FirstOrDefault(l => l.ISBN == isbn)
                        ?? throw new Exception("Livro não encontrado.");
            livro.Disponivel = disponivel;
        }

        public List<Livro> ListarLivros() => livros;
    }

    // Classe de Gerenciamento de Usuários
    public class GerenciadorUsuarios
    {
        private readonly List<Usuario> usuarios = new List<Usuario>();

        public void AdicionarUsuario(int id, string nome)
        {
            if (usuarios.Any(u => u.ID == id))
            {
                throw new Exception("Erro: ID já cadastrado.");
            }
            usuarios.Add(new Usuario { ID = id, Nome = nome });
            Console.WriteLine($"Usuário '{nome}' cadastrado com sucesso!");
        }

        public Usuario BuscarUsuario(int id)
        {
            return usuarios.FirstOrDefault(u => u.ID == id) ?? throw new Exception("Usuário não encontrado.");
        }

        public List<Usuario> ListarUsuarios() => usuarios;
    }

    // Classe de Gerenciamento de Empréstimos
    public class GerenciadorEmprestimos
    {
        private readonly List<Emprestimo> emprestimos = new List<Emprestimo>();
        private readonly INotificacaoService notificacaoService;

        public GerenciadorEmprestimos(INotificacaoService notificacaoService)
        {
            this.notificacaoService = notificacaoService;
        }

        public void RealizarEmprestimo(Usuario usuario, Livro livro, int diasEmprestimo)
        {
            livro.Disponivel = false;
            var emprestimo = new Emprestimo
            {
                Usuario = usuario,
                Livro = livro,
                DataEmprestimo = DateTime.Now,
                DataDevolucaoPrevista = DateTime.Now.AddDays(diasEmprestimo)
            };
            emprestimos.Add(emprestimo);

            notificacaoService.EnviarNotificacao(
                usuario.Nome,
                "Empréstimo Realizado",
                $"Você pegou emprestado o livro: {livro.Titulo}. Devolva até {emprestimo.DataDevolucaoPrevista:dd/MM/yyyy}."
            );

            Console.WriteLine($"Empréstimo realizado para '{usuario.Nome}', livro '{livro.Titulo}'.");
        }

        public double RealizarDevolucao(Usuario usuario, Livro livro)
        {
            var emprestimo = emprestimos.FirstOrDefault(e => e.Usuario.ID == usuario.ID && e.Livro.ISBN == livro.ISBN && e.DataDevolucaoEfetiva == null)
                             ?? throw new Exception("Empréstimo não encontrado.");

            emprestimo.DataDevolucaoEfetiva = DateTime.Now;
            livro.Disponivel = true;

            double multa = 0;
            if (emprestimo.DataDevolucaoEfetiva > emprestimo.DataDevolucaoPrevista)
            {
                var diasAtraso = (emprestimo.DataDevolucaoEfetiva.Value - emprestimo.DataDevolucaoPrevista).Days;
                multa = diasAtraso * 1.0;
            }

            notificacaoService.EnviarNotificacao(
                usuario.Nome,
                "Devolução Realizada",
                multa > 0
                    ? $"Obrigado por devolver o livro '{livro.Titulo}'. Você tem uma multa de R$ {multa:N2} pelo atraso."
                    : $"Obrigado por devolver o livro '{livro.Titulo}' dentro do prazo!"
            );

            Console.WriteLine($"Devolução realizada para '{usuario.Nome}', livro '{livro.Titulo}'. Multa: R$ {multa:N2}");
            return multa;
        }
    }

    // Modelos
    public class Livro
    {
        public string Titulo { get; set; }
        public string Autor { get; set; }
        public string ISBN { get; set; }
        public bool Disponivel { get; set; }
    }

    public class Usuario
    {
        public int ID { get; set; }
        public string Nome { get; set; }
    }

    public class Emprestimo
    {
        public Usuario Usuario { get; set; }
        public Livro Livro { get; set; }
        public DateTime DataEmprestimo { get; set; }
        public DateTime DataDevolucaoPrevista { get; set; }
        public DateTime? DataDevolucaoEfetiva { get; set; }
    }

    // Classe principal
    public class Program
    {
        public static void Main(string[] args)
        {
            var notificacaoService = new NotificacaoService();
            var gerenciadorLivros = new GerenciadorLivros();
            var gerenciadorUsuarios = new GerenciadorUsuarios();
            var gerenciadorEmprestimos = new GerenciadorEmprestimos(notificacaoService);

            // Adicionar livros
            gerenciadorLivros.AdicionarLivro("Clean Code", "Robert C. Martin", "978-0132350884");

            // Adicionar usuários
            gerenciadorUsuarios.AdicionarUsuario(1, "João Silva");

            // Realizar empréstimo
            var usuario = gerenciadorUsuarios.BuscarUsuario(1);
            var livro = gerenciadorLivros.BuscarLivro("978-0132350884");
            gerenciadorEmprestimos.RealizarEmprestimo(usuario, livro, 7);

            // Realizar devolução
            gerenciadorEmprestimos.RealizarDevolucao(usuario, livro);
        }
    }
}
