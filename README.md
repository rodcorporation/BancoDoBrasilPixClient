# BancoDoBrasilPixClient
Biblioteca que implementa o PIX do banco do brasil.

## NuGet Packages

| **Package** | **Latest Version** | **About** |
|:--|:--|:--|
| `BancoDoBrasilPixClient` | [![NuGet](https://buildstats.info/nuget/BancoDoBrasilPixClient)](https://www.nuget.org/packages/BancoDoBrasilPixClient "Download BancoDoBrasilPixClient from NuGet.org") | Versão base com ambiente de produção e sandbox |

## Começo rápido

Para utilizar a biblioteca PIX do Banco do Brasil, obtenha em mãos as credenciais de Teste e de Produção se registrando no site oficial do Banco do Brail para Desenvolvedores(https://www.bb.com.br/site/developers/).

A biblioteca permite você utilizar o tanto o ambiente de teste (utilizando o EnvironmentType.Testing) e de produção (EnvironmentType.Production).

Vale ressaltar que cada ambiente tem suas próprias credencias, uma não permite ser usada na outra e vice-versa.

### Autenticação

<!-- snippet: quick-start -->
```cs
// Cria o cliente de comunicação para criação ou consulta das cobranças PIX geradas.
PixClient client = new PixClient(EnvironmentType.Testing,
                                 "Meu ClienteId",
                                 "Meu ClienteSecret",
                                 "Minha ApplicationKey");

// Autentica no serviço.
client.Autenticar(PixClient.Scopes.AllScopes);
```
<!-- endSnippet -->
