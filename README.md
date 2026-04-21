## Visão Geral

Jogatinas é um sistema web de votação e gestão de sessões de jogos.

---
## Funcionalidades Atuais
### Jogos
- Qualquer usuário autenticado pode cadastrar jogos na lista
- Campos disponíveis: título (obrigatório), gênero, ano de lançamento, ~~URL da imagem~~, URL da página na loja
- A URL da loja é validada para aceitar apenas domínios confiáveis, sendo eles, atualmente: `store.steampowered.com`, `epicgames.com`, `gog.com`, `microsoft.com`, `xbox.com`, `nintendo.com`, `playstation.com`, `ubisoft.com`, `ea.com`, `battle.net`, `itch.io`
- Administradores podem remover jogos da lista
### Votação
- Cada usuário autenticado pode votar em quantos jogos quiser
- É permitido apenas um voto por usuário por jogo
- O usuário pode remover o próprio voto
- Usuários não autenticados visualizam a lista mas são redirecionados ao login ao tentar votar
### Lista Principal
- Exibe jogos ordenados pela data do último voto recebido, ou seja, o jogo com o voto mais recente aparece acima
- Jogos agendados para uma sessão são removidos da lista principal e exibidos em destaque
- O tamanho da lista é configurável por usuários autenticados
### Mais Votados
- Visão secundária que exibe jogos ordenados pelo total de votos
- Em caso de empate, o desempate é feito pela data do primeiro voto, o jogo que recebeu votos mais cedo aparece acima
- O tamanho da lista é configurável por usuários autenticados
### Sessões Agendadas
- Usuários autenticados podem agendar uma data para jogar um jogo da lista
- O jogo agendado é destacado na página principal e removido da lista de votação
- As sessões do dia atual recebem destaque visual diferente das outras
- Usuários autenticados podem editar a data de uma sessão ou cancelá-la. Ao cancelar, o jogo retorna à lista principal
- O tamanho da lista de agendamentos é configurável por usuários autenticados
### Histórico
- Ao marcar uma sessão como jogada, os votos do jogo são resetados e o registro é movido para o histórico de sessões
- Pode ser informada a data real em que o jogo foi jogado, incluindo datas passadas
- O título do jogo é preservado no histórico mesmo que o jogo seja removido da lista
- O tamanho do histórico é configurável por usuários autenticados
### Configurações
- Usuários autenticados têm acesso a um painel de configurações onde podem definir o número de itens exibidos em cada lista
### Busca
- Barra de busca em tempo real que filtra jogos pelo título na lista ativa
---
## Papéis de Usuário

| Papel         | Permissões                                                                                   |
| ------------- | -------------------------------------------------------------------------------------------- |
| Visitante     | Visualizar listas e histórico                                                                |
| Membro        | Votar, remover voto, cadastrar jogos, agendar sessões, marcar como jogado, configurar listas |
| Administrador | Tudo acima + remover jogos                                                                   |
|               |                                                                                              |
|               |                                                                                              |

--- 
## Regras de Negócio

1. Um usuário pode votar em múltiplos jogos, mas apenas uma vez por jogo
2. Jogos agendados não aparecem na lista de votação
3. Ao marcar uma sessão como jogada, todos os votos daquele jogo são resetados
4. A lista principal é ordenada pela data do último voto
5. O desempate na visão mais votados é feito pelo primeiro voto recebido
6. A URL da loja é validada por domínio, domínios não permitidos são rejeitados
7. O histórico preserva o título do jogo independente de o jogo existir na lista
---
## Roadmap
### Em avaliação
- **Restrição de registro**: limitar o cadastro apenas a membros de uma comunidade existente, possivelmente por convite ou validação via Steam/Discord
- **Integração com API de catálogo**: buscar automaticamente dados de jogos (capa, gênero, ano) ao cadastrar, usando uma API com limite gratuito
- **Alertas aos usuários**: notificações quando uma sessão for agendada e/ou no dia ou dia anterior à sessão
- **Dashboards**: painéis com informações como jogos mais jogados
### Planejado
- **Edição de jogos**: permitir que administradores editem informações de jogos cadastrados
- **Otimização das regras da lista principal**: ajustar critérios de ordenação conforme feedback
---
## Observações

O sistema inteiro é um experimento que deve evoluir ao longo do tempo, com novas regras de negócio e funcionalidades.

---

# Documentação Técnica
## Stack

| Camada           | Tecnologia              |
| ---------------- | ----------------------- |
| Linguagem        | C# 13                   |
| Plataforma       | .NET 10                 |
| Framework web    | ASP.NET Core MVC 9      |
| ORM              | Entity Framework Core 9 |
| Banco de dados   | SQLite                  |
| Autenticação     | ASP.NET Core Identity   |
| Template engine  | Razor                   |
| CSS framework    | Bootstrap 5.3           |
| Ícones           | Bootstrap Icons 1.11    |
| Testes unitários | xUnit + Moq             |
|                  |                         |

--- 
## Arquitetura

O projeto segue uma arquitetura em camadas dentro de um único projeto ASP.NET Core MVC, com separação clara de responsabilidades orientada pelos princípios SOLID e Clean Code.

```
Controllers → Services → Repositories → DbContext → SQLite
```

Cada camada depende apenas da camada seguinte através de interfaces, seguindo o Dependency Inversion Principle. O registro das dependências é feito via injeção de dependência nativa do ASP.NET Core no `Program.cs`.

---
## Estrutura de Pastas
 
```
GameVoting/
├── Controllers/            ← recebe requisições HTTP e delega para Services
├── Data/
│   └── AppDbContext.cs     ← contexto do EF Core + Identity
├── Models/
│   ├── Entities/           ← entidades do banco de dados
│   ├── Validation/         ← atributos de validação customizados
│   └── ViewModels/         ← modelos específicos para as Views
├── Repositories/
│   ├── Interfaces/         ← contratos dos repositórios
│   └── *Repository.cs      ← implementações de acesso ao banco
├── Services/
│   ├── Interfaces/         ← contratos dos serviços
│   └── *Service.cs         ← regras de negócio
├── Views/                  ← arquivos Razor (.cshtml)
│   ├── Shared/             ← layouts parciais reutilizáveis
│   └── */                  ← views por controller
└── wwwroot/                ← arquivos estáticos (CSS, JS)
 
GameVoting.Tests/
├── Services/               ← testes unitários das Services
└── Validation/             ← testes unitários dos Validators
```
 
---
## Modelo de Dados
### Entidades

**`Game`** — representa um jogo na lista de votação.

| Campo | Tipo | Descrição |
|---|---|---|
| Id | int | Chave primária |
| Title | string | Título do jogo |
| Genre | string? | Gênero |
| ReleaseYear | int? | Ano de lançamento |
| ImageUrl | string? | URL da capa |
| StorePageUrl | string? | URL da página na loja |
| AddedAt | DateTime | Data de cadastro |

**`Vote`** — representa um voto de um usuário em um jogo.

| Campo | Tipo | Descrição |
|---|---|---|
| Id | int | Chave primária |
| GameId | int | FK → Game |
| UserId | string | FK → AspNetUsers |
| VotedAt | DateTime | Data e hora do voto (UTC) |

Índice único em `(UserId, GameId)` — garante um voto por usuário por jogo no nível do banco.
 
**`GameSession`** — representa uma sessão agendada ou realizada.

| Campo         | Tipo      | Descrição                                         |
| ------------- | --------- | ------------------------------------------------- |
| Id            | int       | Chave primária                                    |
| GameId        | int?      | FK → Game (nullable — SetNull ao deletar jogo)    |
| GameTitle     | string?   | Título preservado independente do jogo existir    |
| ScheduledDate | DateTime  | Data agendada                                     |
| PlayedAt      | DateTime? | Data em que foi jogado — nulo se ainda não jogado |

**`SiteSettings`** — configurações globais da aplicação. Sempre existe exatamente um registro.

| Campo            | Tipo | Padrão |
| ---------------- | ---- | ------ |
| MainListSize     | int  | 20     |
| TopListSize      | int  | 20     |
| HistoryListSize  | int  | 10     |
| ScheduleListSize | int  | 10     |

**`ApplicationUser`** — estende o `IdentityUser` do ASP.NET Core Identity.

| Campo | Tipo | Descrição |
|---|---|---|
| DisplayName | string | Nome de exibição |
| RegisteredAt | DateTime | Data de registro |

---
## Camada de Serviços

Os Services concentram todas as regras de negócio. Controllers nunca acessam repositórios diretamente.
 
**`GameService`**: gerencia a lista de jogos e monta o `GameIndexViewModel` com as quatro listas da página principal (ranking recente, mais votados, agendamentos e histórico), aplicando filtros, ordenações e limites configuráveis.

**`VoteService`**: gerencia votos. Garante a regra de um voto por usuário por jogo antes de persistir.

**`GameSessionService`**: gerencia sessões. Ao marcar como jogado, reseta os votos do jogo e registra a data. Ao cancelar, remove a sessão devolvendo o jogo à lista principal.

---
## Autenticação e Autorização

Autenticação via ASP.NET Core Identity com cookie persistente. Dois papéis definidos:
- **Member** — usuários registrados
- **Admin** — acesso a funcionalidades administrativas
O papel Admin é atribuído manualmente via banco de dados ou seed. As actions administrativas são protegidas com `[Authorize(Roles = "Admin")]`.
---
## Validações

Validações de ViewModel via Data Annotations (`[Required]`, `[Range]`, `[EmailAddress]`, etc.), aplicadas tanto no servidor quanto no client via Tag Helpers do Razor.

Validação customizada via atributo `StoreUrlAttribute` valida que a URL informada pertence a um domínio de loja permitido. Atualmente aceita `store.steampowered.com`, `epicgames.com`, `gog.com`, `microsoft.com`, `xbox.com`, `nintendo.com`, `playstation.com`, `ubisoft.com`, `ea.com`, `battle.net`, `itch.io`. Extensível via array `AllowedDomains`.

--- 
## Testes

Testes unitários dos Services com xUnit e Moq. Os repositórios são mockados via interfaces, sem dependência de banco de dados.

Cobertura atual: 100% de linha e branch nas três Services (`GameService`, `VoteService`, `GameSessionService`).

Testes de integração não implementados. Planejados para versão futura usando `WebApplicationFactory` com SQLite em memória.

--- 
## Observações

**Arquitetura em camadas com interfaces**: a separação em Controllers, Services e Repositories, com dependências sempre por interface, foi adotada desde o início para garantir aderência ao SOLID, facilitar testes unitários e permitir substituição de implementações sem impacto nas camadas superiores.

**SQLite**: o banco é um arquivo único, sem servidor. A troca para SQL Server ou PostgreSQL em produção requer apenas alteração do provider no `Program.cs`, sem impacto no restante do código.

**EF Core com Migrations**: todo o esquema do banco é gerenciado via migrations, garantindo rastreabilidade e reprodutibilidade do ambiente.

--- 
## Roadmap Técnico
- Implementar testes de integração com `WebApplicationFactory`
- Avaliar integração com API de catálogo de jogos (RAWG)
- Configurar pipeline de CI com execução automática de testes
- Definir estratégia de hospedagem e banco de dados em produção
- Implementar roles de Admin com atribuição via interface ou seed