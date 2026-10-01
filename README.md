# Produção SIG

## Publicação de versões

Consulte o [tutorial de atualizações](docs/TUTORIAL_ATUALIZACOES.md) para gerar os pacotes, enviar um rascunho à Central SIG, homologar e liberar em produção.

No PowerShell 7, após atualizar `Version`, `AssemblyVersion` e `FileVersion` em `Producao/Producao.csproj`:

```powershell
.\deploy.ps1 -Changelog @('Descreva as alterações desta versão')
```

Para apenas gerar os arquivos, acrescente `-SkipServerUpload`. O envio cria um rascunho; a liberação é feita no painel da Central.
