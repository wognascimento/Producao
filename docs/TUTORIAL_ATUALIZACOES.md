# Produção SIG — como publicar uma nova versão

Atualizado em 01/10/2026.

**Central:** https://atualizasig.cipolatti.com.br

O fluxo é: **alterar a versão → gerar os arquivos → enviar como rascunho → homologar → publicar em produção**. O script não libera automaticamente uma versão para os usuários.

## 1. Preparação da máquina de desenvolvimento

Você precisa de:

- PowerShell **7 ou superior** (`pwsh`). O Windows PowerShell 5.1 não oferece o envio multipart usado pelo script.
- SDK .NET 10 e acesso às dependências do projeto, incluindo os pacotes Telerik.
- Inno Setup 6. O script procura `tools\InnoSetup6\ISCC.exe` no workspace e, em seguida, a instalação padrão. Também aceita `-InnoCompiler`.
- Instalador oficial **.NET Desktop Runtime 10 x64** em `tools\dotnet\windowsdesktop-runtime-10.0.X-win-x64.exe`. Use um instalador atualizado da Microsoft. Se houver mais de um, o script seleciona a maior versão local.
- Conta administrativa da Central. É a conta do painel, não a conta SSH do servidor.

Abra o PowerShell 7 e entre no projeto:

```powershell
Set-Location 'C:\Users\admin\Documents\Codex\2026-05-21\clona-o-repositorio-wognascimento-financeiro-git\Producao'
$PSVersionTable.PSVersion
dotnet --version
```

Os comandos seguintes pressupõem que você está nessa pasta.

## 2. Escolha uma versão nova

No painel, confira **Versões → Produção**. Considere também versões em rascunho, homologação e suspensas: um número já enviado não pode ser reutilizado.

Edite `Producao\Producao.csproj` e atualize os três campos. Exemplo de uma próxima versão, desde que `2.0.0.1` ainda não exista:

```xml
<Version>2.0.0.1</Version>
<AssemblyVersion>2.0.0.1</AssemblyVersion>
<FileVersion>2.0.0.1</FileVersion>
```

O script verifica se esses campos concordam e se a DLL compilada tem a versão esperada. **Se você alterar o conteúdo após enviar uma versão, use outro número**, por exemplo `2.0.0.2`.

> Na validação desta integração foram gerados artefatos locais `2.0.0.0`, sem envio à Central. Eles não representam uma nova versão publicada. Para a próxima entrega real, escolha uma versão superior à instalada e ainda não cadastrada.

## 3. Gere e envie o rascunho

```powershell
.\deploy.ps1 -Changelog @(
    'Corrige a validação de quantidade no estoque'
    'Melhora a consulta do checklist'
)
```

Informe o usuário e a senha da **Central SIG** quando solicitado. A senha não fica gravada no script nem nos artefatos.

O script:

1. Confere a versão e autentica na Central.
2. Verifica se a versão já existe, antes da compilação.
3. Publica o aplicativo para Windows x64 e confere os arquivos do atualizador.
4. Gera o instalador Inno Setup e o ZIP.
5. Salva os arquivos e seus hashes em uma pasta própria da versão.
6. Envia ZIP, instalador e notas à API, criando um **rascunho**.
7. Salva o recibo do envio e encerra a sessão.

Para o exemplo `2.0.0.1`, os arquivos ficam em:

```text
artifacts\versions\2.0.0.1\
  application-2.0.0.1.zip
  ProducaoSetup-2.0.0.1.exe
  release.json
  publication.json                 # criado após o envio confirmado
```

| Arquivo | Finalidade |
|---|---|
| ZIP | Atualização executada pelo `Update.exe` dos clientes |
| EXE | Instalação inicial, primeira migração ou reinstalação completa |
| `release.json` | Metadados locais, alterações, tamanho e SHA-256 dos artefatos |
| `publication.json` | Identificador do rascunho criado na Central |

**Não envie `version.json` manualmente.** A Central gera o manifesto, as URLs de download, os hashes e a assinatura ao disponibilizar o canal.

### Gerar sem enviar

Para revisar os arquivos antes de conectar ao servidor:

```powershell
.\deploy.ps1 -SkipServerUpload -Changelog @(
    'Corrige a validação de quantidade no estoque'
    'Melhora a consulta do checklist'
)
```

Nenhum arquivo é enviado à Central ou ao compartilhamento. Depois, envie os mesmos arquivos, sem recompilar:

```powershell
.\deploy.ps1 -UploadOnly -ArtifactDirectory '.\artifacts\versions\2.0.0.1'
```

O envio usa as notas e a versão mínima salvas em `release.json`, verificando os hashes antes de transmitir. Não edite o ZIP/EXE nessa pasta; se o conteúdo mudou, gere uma versão nova.

### Enviar manualmente pelo painel

Após gerar com `-SkipServerUpload`:

1. Abra a Central por HTTPS e entre na conta.
2. Clique em **Nova versão**.
3. Selecione **Produção**, informe a versão e a versão mínima compatível.
4. Copie as alterações registradas em `release.json`.
5. Selecione o ZIP em **Pacote de atualização ZIP**.
6. Selecione o EXE em **Instalador EXE**.
7. Clique em **Validar e salvar rascunho**.

Escolha apenas um dos caminhos: envio pelo script ou pelo painel. Não tente cadastrar a mesma versão duas vezes.

## 4. Homologue em uma estação de testes

No painel, abra **Versões → Produção → Detalhes → Homologar**.

Manifesto do canal de teste:

```text
https://atualizasig.cipolatti.com.br/downloads/producao/homologacao/version.json
```

O Produção ajustado aceita a variável `PRODUCAO_UPDATE_URL`, que tem prioridade sobre a chave `UpdateInfoUrl` do arquivo de configuração e sobre a URL padrão.

Para usar o canal de homologação **somente no processo de teste**, feche o Produção, abra um PowerShell e execute:

```powershell
$env:PRODUCAO_UPDATE_URL = 'https://atualizasig.cipolatti.com.br/downloads/producao/homologacao/version.json'
Start-Process 'C:\SIG\Producao S.I.G\Producao.exe'
```

Adapte o caminho se a instalação estiver em outra pasta. No programa, use **Outros → Atualizar Sistema**.

Confira:

- Número instalado em **Outros → Sobre**.
- Download, fechamento, instalação e reabertura do programa.
- Login e conexão com o banco da estação de testes.
- As telas e operações alteradas pela versão.
- Relatórios/modelos utilizados pelos usuários.

O atualizador só oferece uma versão **maior** que a instalada. Para testar o fluxo de atualização ZIP, a estação precisa de uma versão anterior que reconheça a configuração do novo servidor. Instalar o EXE da mesma versão permite testar a instalação e as funcionalidades, mas não comprova uma atualização ZIP.

### Primeira migração dos clientes antigos

As instalações antigas ainda podem consultar `192.168.0.49` e não reconhecer `PRODUCAO_UPDATE_URL`. Para a primeira migração, instale o **novo EXE** em uma estação piloto, configure o canal desejado e valide o sistema. Depois de publicar em produção, use o instalador aprovado nas demais estações conforme o processo da TI.

O novo instalador e o ZIP não levam os arquivos locais `Producao.dll.config`, `Producao.exe.config` ou `App.config`. Configurações já existentes são preservadas. Verifique se a chave `UpdateInfoUrl` existente ainda aponta para o servidor antigo; remova apenas essa chave ou altere seu valor para o manifesto novo, preservando as configurações de banco.

Na implantação inicial em uma máquina nova, mantenha o provisionamento habitual das configurações de banco do SIG. A Central não configura credenciais do banco.

**Biblioteca compartilhada e atualizador:** o atualizador legado ignora a substituição de `BibliotecasSIG.dll` e dos seus próprios arquivos. Se uma entrega alterar esses componentes, distribua o instalador completo; não considere o ZIP suficiente até que esse mecanismo seja revisado.

## 5. Publique em produção

Após validar na estação piloto:

1. Abra a versão na Central.
2. Clique em **Publicar em produção**.
3. Confirme que a versão foi testada.
4. Confira o manifesto:

```text
https://atualizasig.cipolatti.com.br/downloads/producao/version.json
```

Para consultar pelo PowerShell:

```powershell
Invoke-RestMethod 'https://atualizasig.cipolatti.com.br/downloads/producao/version.json' |
    Select-Object updateVersion, updateUrl, installerUrl, releaseDate
```

Os clientes consultam **Outros → Atualizar Sistema**. A verificação automática na inicialização não foi ativada nesta alteração.

Para tirar a estação de testes do canal de homologação, feche o aplicativo. Se usou apenas `$env:PRODUCAO_UPDATE_URL` no terminal, remova a variável nesse terminal antes de reabrir o programa:

```powershell
Remove-Item Env:\PRODUCAO_UPDATE_URL -ErrorAction SilentlyContinue
```

Se você configurou a variável permanentemente no Windows, no AD ou em um atalho, ajuste também essa origem. Sem variável nem chave `UpdateInfoUrl` personalizada, o novo código usa produção na Central.

## 6. Copie o instalador para a rede, se necessário

A geração/envio do rascunho **não copia automaticamente** o instalador para a rede. Depois da publicação em produção:

```powershell
.\deploy.ps1 -CopyPublishedInstaller `
    -ArtifactDirectory '.\artifacts\versions\2.0.0.1' `
    -NetworkDeployPath '\\192.168.0.4\sistemas\SIG'
```

O script confere a versão e o hash do instalador no manifesto de produção antes de copiar. O diretório de rede deve existir e permitir gravação. Se precisar de outra conta para o compartilhamento:

```powershell
$rede = Get-Credential -Message 'Conta de acesso ao compartilhamento'
.\deploy.ps1 -CopyPublishedInstaller `
    -ArtifactDirectory '.\artifacts\versions\2.0.0.1' `
    -NetworkDeployPath '\\192.168.0.4\sistemas\SIG' `
    -NetworkCredential $rede
```

## 7. Falhas e correções

| Situação | Como agir |
|---|---|
| Versão já cadastrada | Use **Detalhes** da versão existente. Para outro conteúdo, aumente os três campos de versão e gere novamente. Não existe publicação forçada. |
| Pasta local da versão já existe | Se os arquivos estão prontos, use `-UploadOnly`. Se houve mudança no código, use uma versão nova. |
| Conexão caiu durante o envio | Confira primeiro se o rascunho apareceu na Central. Se apareceu, continue pelo painel; se não apareceu, use `-UploadOnly` com a mesma pasta. |
| Hash/tamanho divergente | O arquivo mudou ou ficou incompleto. Não edite `release.json` para contornar a validação. Gere novamente a partir do código correto com uma versão nova. |
| `404` em `version.json` | Ainda não existe versão publicada naquele canal, ou a versão foi suspensa. Rascunhos não geram manifesto público. |
| Erro ao entrar no painel/API | Use HTTPS e a conta do painel. Se o acesso foi recém-configurado, recarregue com Ctrl+F5. |
| Erro na compilação/restauração | Confira SDK e feeds NuGet/Telerik. O script preserva a pasta `artifacts\.staging\...` para diagnóstico. |
| Falta do runtime na estação | O instalador padrão inclui o runtime e pede elevação apenas quando precisa instalá-lo. Se a instalação do runtime falhar, ele bloqueia a instalação do aplicativo com a mensagem correspondente. |
| Atualização não aparece | Compare a versão instalada com o manifesto e verifique o canal, a variável de ambiente e a chave `UpdateInfoUrl`. |
| Problema após liberar uma versão | Use **Suspender** na Central e publique uma correção com número maior. Suspender não desfaz instalações já concluídas nem migrações de banco. |

## Parâmetros úteis

| Parâmetro | Uso |
|---|---|
| `-Changelog @('Alteração 1','Alteração 2')` | Notas obrigatórias ao gerar novos artefatos |
| `-MinimumCompatibleVersion '2.0.0.0'` | Registra a versão mínima esperada; padrão `1.0.0.0` |
| `-SkipServerUpload` | Apenas gera arquivos locais |
| `-UploadOnly -ArtifactDirectory '...'` | Envia arquivos já gerados, sem recompilar |
| `-Credential (Get-Credential)` | Fornece a conta do painel ao script |
| `-DotNetDesktopRuntimeInstallerPath 'C:\...\runtime.exe'` | Define o instalador oficial do runtime |
| `-SkipRuntimeBundle` | Gera instalador sem runtime; exige runtime já instalado na estação |
| `-InnoCompiler 'C:\...\ISCC.exe'` | Define o compilador Inno Setup |
| `-CopyPublishedInstaller` | Copia para a rede somente um instalador publicado em produção |

`-SkipNetworkCopy` foi mantido por compatibilidade, mas a cópia já é opcional. `-ForceDeploy`, `-ServerUploadPath` e `-UpdateBaseUrl` são recusados com orientação para o fluxo novo.

O atualizador legado ainda não aplica `minimumCompatibleVersion`, hash ou assinatura do manifesto. Homologação e compatibilidade precisam ser verificadas antes da publicação. Esta entrega integra o processo de geração/envio; não reimplementa a instalação nos clientes.

## Validação desta alteração

- Geração real do Produção para Windows x64 concluída; ZIP e instalador criados localmente.
- Inno Setup compilou a versão com runtime incorporado.
- Testes de contrato da publicação cobrem HTTPS, renovação de CSRF após login, duplicidade inclusive versões suspensas, hashes, arquivos do multipart, recibo sem credenciais e encerramento da sessão.
- Nenhuma versão foi enviada à Central ou ao compartilhamento durante a validação.
- A compilação do aplicativo mantém avisos existentes. A execução do instalador numa estação Windows e a homologação funcional com o banco devem ser feitas antes da distribuição.

Referências: [instalação do .NET no Windows](https://learn.microsoft.com/en-us/dotnet/core/install/windows), [arquivos do Inno Setup](https://jrsoftware.org/ishelp/topic_filessection.htm), [privilégios do instalador](https://jrsoftware.org/ishelp/topic_setup_privilegesrequired.htm).
