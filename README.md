1. Mistura de responsabilidades
Classe e Método: GerenciadorBiblioteca, métodos EnviarEmail e EnviarSMS.
Princípio Violado: Single Responsibility Principle (SRP).

Porque: A classe GerenciadorBiblioteca mistura responsabilidades ao gerenciar operações relacionadas à biblioteca e ao envio de notificações. Isso viola o SRP, que estabelece que cada classe deve ter apenas uma razão para mudar. Idealmente, o envio de e-mails e SMS deveria ser tratado por uma classe dedicada, como ServicoDeNotificacao, evitando a quebra deste princípio e facilitando a manutenção do código.

2. Falta de validação de entrada
Classe e Método: GerenciadorBiblioteca, métodos AdicionarLivro e AdicionarUsuario.
Princípio Violado: Clean Code - Validação de entrada.

Porque: Os métodos não validam os dados antes de adicioná-los à lista, permitindo, por exemplo, duplicatas com o mesmo ISBN ou ID. A falta de validação torna o código menos polido e aumenta o risco de inconsistências no sistema. Incluir validações melhora a legibilidade, confiabilidade e organiza o fluxo lógico do código.

3. Tratamento de null safety em RealizarEmprestimo
Classe e Método: GerenciadorBiblioteca, método RealizarEmprestimo.
Princípio Violado: Clean Code - Mensagens de erro claras.

Porque: O método utiliza Find para procurar um livro e um usuário, mas não valida se as variáveis livro ou usuario são null antes de acessar suas propriedades ou métodos. Apesar disso ser tratado implicitamente, é considerado uma má prática porque mensagens de erro claras tornam o código mais robusto, prevenindo falhas e facilitando a depuração.

4. Cálculo de multa com baixa precisão
Classe e Método: GerenciadorBiblioteca, método RealizarDevolucao.
Princípio Violado: Clean Code - Precisão em cálculos.

Porque: O cálculo da multa utiliza Days, que retorna apenas dias inteiros. Isso pode não ser adequado para cenários onde seja necessária maior precisão, como incluir horas de atraso no cálculo. Melhorar a precisão utilizando métodos que considerem horas pode garantir resultados mais exatos e satisfatórios.

5. Método RealizarDevolucao com lógica complexa
Classe e Método: GerenciadorBiblioteca, método RealizarDevolucao.
Princípio Violado: Clean Code - Funções pequenas e focadas.

Porque: Este método realiza múltiplas tarefas, como localizar um empréstimo, atualizar o estado do livro e calcular multas. Isso vai contra o princípio de criar funções que realizam apenas uma tarefa específica. Dividir o método em partes menores facilita a compreensão e manutenção, além de seguir boas práticas de código limpo.

6. Violação do Open/Closed Principle (OCP)
Classe e Método: GerenciadorBiblioteca, métodos relacionados a notificações (EnviarEmail e EnviarSMS).
Princípio Violado: Open/Closed Principle (OCP).

Porque: A classe não está aberta para extensões e fechada para modificações. Se for necessário adicionar outro tipo de notificação, como push notifications, será preciso alterar diretamente a classe, o que vai contra o princípio OCP. Delegar essas tarefas a uma abstração, como a interface INotificacaoService, tornaria o código mais flexível e fácil de estender.


7. Dependência direta de implementação
Classe e Método: GerenciadorBiblioteca, métodos que utilizam listas como livros, usuarios e emprestimos.
Princípio Violado: Dependency Inversion Principle (DIP).

Porque: A classe depende diretamente das listas concretas (List<Livro>, List<Usuario>, etc.) em vez de abstrações. Isso torna o código rígido e difícil de adaptar, por exemplo, para integração com banco de dados. Utilizar abstrações permite maior flexibilidade e desacoplamento da implementação.
