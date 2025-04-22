using System;
using System.Collections.Generic;
using System.Linq;

namespace BibliotecaApp
{
    // Interface para enviar notificações
    public interface INotificacaoService
    {
        void EnviarEmail(string destinatario, string assunto, string mensagem);
        void EnviarSMS(string destinatario, string mensagem);
    }

    // Implementação do serviço de notificações
    public class NotificacaoService : INotificacaoService
    {
        public void EnviarEmail(string destinatario, string assunto, string mensagem)
        {
            Console.WriteLine($"E-mail enviado para {destinatario}. Assunto: {assunto}");
        }

        public void EnviarSMS(string destinatario, string mensagem)
        {
            Console.WriteLine($"SMS enviado para {destinatario}: {mensagem}");
        }
    }

    // Tipos de Usuário
    public abstract class Usuario
    {
        public string Nome { get; set; }
        public int ID { get; set; }
    }

    public class UsuarioComum : Usuario
    {
        public string Endereco { get; set; }
    }

    public class Funcionario : Usuario
    {
        public string Cargo { get; set; }
    }

    // Interface para gerenciar biblioteca
    public interface IBibliotecaService
    {
        void AdicionarLivro(string titulo, string autor, string isbn);
        void AdicionarUsuario(Usuario usuario);
        bool RealizarEmprestimo(int usuarioId, string isbn, int diasEmprestimo);
        double RealizarDevolucao(string isbn, int usuarioId);
        List<Livro> BuscarTodosLivros();
        List<Usuario> BuscarTodosUsuarios();
        List<Funcionario> BuscarFuncionarios();
        List<UsuarioComum> BuscarUsuariosComuns();
    }

    // Implementação do gerenciamento da biblioteca
    public class BibliotecaService : IBibliotecaService
    {
        private readonly List<Livro> livros = new List<Livro>();
        private readonly List<UsuarioComum> usuariosComuns = new List<UsuarioComum>();
        private readonly List<Funcionario> funcionarios = new List<Funcionario>();
        private readonly INotificacaoService notificacaoService;

        public BibliotecaService(INotificacaoService notificacaoService)
        {
            this.notificacaoService = notificacaoService;
        }

        public void AdicionarLivro(string titulo, string autor, string isbn)
        {
            if (livros.Exists(l => l.ISBN == isbn))
            {
                throw new Exception("Erro: ISBN já cadastrado.");
            }

            var livro = new Livro
            {
                Titulo = titulo,
                Autor = autor,
                ISBN = isbn,
                Disponivel = true
            };
            livros.Add(livro);
        }

        public void AdicionarUsuario(Usuario usuario)
        {
            if (usuario is UsuarioComum comum)
            {
                if (usuariosComuns.Exists(u => u.ID == comum.ID))
                {
                    throw new Exception("Erro: ID de usuário comum já cadastrado.");
                }
                usuariosComuns.Add(comum);
                notificacaoService.EnviarEmail(comum.Nome, "Bem-vindo à Biblioteca", "Cadastro realizado com sucesso!");
            }
            else if (usuario is Funcionario funcionario)
            {
                if (funcionarios.Exists(f => f.ID == funcionario.ID))
                {
                    throw new Exception("Erro: ID de funcionário já cadastrado.");
                }
                funcionarios.Add(funcionario);
                notificacaoService.EnviarEmail(funcionario.Nome, "Bem-vindo à equipe", "Você foi cadastrado como funcionário da biblioteca!");
            }
        }

        public bool RealizarEmprestimo(int usuarioId, string isbn, int diasEmprestimo)
        {
            var livro = livros.Find(l => l.ISBN == isbn);
            var usuario = usuariosComuns.Find(u => u.ID == usuarioId) as Usuario ?? funcionarios.Find(f => f.ID == usuarioId);

            if (livro == null)
            {
                throw new Exception("Erro: Livro não encontrado.");
            }

            if (usuario == null)
            {
                throw new Exception("Erro: Usuário não encontrado.");
            }

            if (!livro.Disponivel)
            {
                throw new Exception("Erro: Livro indisponível.");
            }

            livro.Disponivel = false;
            notificacaoService.EnviarEmail(usuario.Nome, "Empréstimo Realizado", $"Você pegou emprestado o livro: {livro.Titulo}");
            notificacaoService.EnviarSMS(usuario.Nome, $"Empréstimo do livro: {livro.Titulo}");

            return true;
        }

        public double RealizarDevolucao(string isbn, int usuarioId)
        {
            var livro = livros.Find(l => l.ISBN == isbn);
            if (livro == null || livro.Disponivel)
            {
                throw new Exception("Erro: Livro já devolvido ou não encontrado.");
            }

            livro.Disponivel = true;
            notificacaoService.EnviarEmail($"Usuário {usuarioId}", "Livro Devolvido", "Obrigado por devolver o livro!");

            return 0; // Sem multa neste exemplo.
        }

        public List<Livro> BuscarTodosLivros() => livros;

        public List<Usuario> BuscarTodosUsuarios() => usuariosComuns.Cast<Usuario>().Concat(funcionarios.Cast<Usuario>()).ToList();

        public List<Funcionario> BuscarFuncionarios() => funcionarios;

        public List<UsuarioComum> BuscarUsuariosComuns() => usuariosComuns;
    }

    public class Livro
    {
        public string Titulo { get; set; }
        public string Autor { get; set; }
        public string ISBN { get; set; }
        public bool Disponivel { get; set; }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var notificacaoService = new NotificacaoService();
            var bibliotecaService = new BibliotecaService(notificacaoService);

            try
            {
                // Adicionar livros
                bibliotecaService.AdicionarLivro("Clean Code", "Robert C. Martin", "978-0132350884");

                // Adicionar usuários comuns
                bibliotecaService.AdicionarUsuario(new UsuarioComum { Nome = "João Silva", ID = 1, Endereco = "Rua A, 123" });
                bibliotecaService.AdicionarUsuario(new UsuarioComum { Nome = "Maria Oliveira", ID = 2, Endereco = "Rua B, 456" });

                // Adicionar funcionários
                bibliotecaService.AdicionarUsuario(new Funcionario { Nome = "Carlos Costa", ID = 1001, Cargo = "Bibliotecário" });
                bibliotecaService.AdicionarUsuario(new Funcionario { Nome = "Ana Souza", ID = 1002, Cargo = "Assistente" });

                // Realizar empréstimos
                bibliotecaService.RealizarEmprestimo(1, "978-0132350884", 7);

                // Listar todos os usuários
                var todosUsuarios = bibliotecaService.BuscarTodosUsuarios();
                Console.WriteLine("Todos os usuários:");
                todosUsuarios.ForEach(u => Console.WriteLine(u.Nome));

                // Listar funcionários
                var funcionarios = bibliotecaService.BuscarFuncionarios();
                Console.WriteLine("Funcionários:");
                funcionarios.ForEach(f => Console.WriteLine($"{f.Nome} - {f.Cargo}"));

                // Listar usuários comuns
                var usuariosComuns = bibliotecaService.BuscarUsuariosComuns();
                Console.WriteLine("Usuários Comuns:");
                usuariosComuns.ForEach(u => Console.WriteLine($"{u.Nome} - {u.Endereco}"));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro: {ex.Message}");
            }
        }
    }
}
