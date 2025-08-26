# Desafio Backend Microserviços Avanade

Este repositório contém a implementação de um desafio de backend utilizando uma arquitetura baseada em microserviços. O objetivo é demonstrar boas práticas de desenvolvimento, escalabilidade e observabilidade em um ambiente distribuído.

⚠️ **Este projeto está em desenvolvimento. Alterações e melhorias estão sendo realizadas continuamente.**

---

## 📋 Visão Geral

O projeto é composto por múltiplos microserviços desenvolvidos em `.NET 9`, cada um responsável por uma funcionalidade específica. A comunicação entre os serviços é realizada via APIs REST, e o ambiente é orquestrado utilizando Docker Compose.

### Principais Tecnologias Utilizadas:
- **.NET 9**: Framework para desenvolvimento dos microserviços.
- **Docker e Docker Compose**: Para containerização e orquestração.
- **Kong API Gateway**: Gerenciamento de APIs.
- **Prometheus e Grafana**: Monitoramento e visualização de métricas.
- **OpenTelemetry**: Coleta de traces distribuídos.
- **Promtail e Loki**: Centralização e consulta de logs.

---

## 📂 Estrutura do Projeto

- **Sales.WebAPI**: Microserviço responsável pela gestão de vendas.
- **Stock.WebAPI**: Microserviço responsável pelo controle de estoque.
- **observability/**: Configurações para monitoramento e observabilidade.
  - `prometheus.yml`: Configuração do Prometheus.
  - `otel-collector-config.yml`: Configuração do OpenTelemetry Collector.
  - `promtail-config.yml`: Configuração do Promtail.
- **kong/**: Configuração do Kong API Gateway.
- **docker-compose.yaml**: Arquivo para orquestração dos serviços.

---

## 🚀 Como Executar o Projeto

### Pré-requisitos:
- Docker e Docker Compose instalados.
- `.NET 9 SDK` instalado.

### Passos:
1. Clone o repositório:
```bash
git clone git@github.com:JoseLSousa/desafio-backend-microservicos-avanade.git2. Configure as variáveis de ambiente no arquivo `.env`.
```
2. Inicie os serviços com Docker Compose:

```bash
docker compose up --build
```

3. Acesse os serviços:
- **Kong API Gateway**: `http://localhost:8000`
- **Prometheus**: `http://localhost:9090`
- **Grafana**: `http://localhost:3000`

---

## 🛠️ Funcionalidades

- **Sales.WebAPI**:
- Cadastro e consulta de vendas.
- Integração com o microserviço de estoque.

- **Stock.WebAPI**:
- Gerenciamento de produtos e controle de estoque.
- Atualização automática com base nas vendas.

- **Observabilidade**:
- Métricas de desempenho e logs centralizados.
- Traces distribuídos para análise de chamadas entre microserviços.

---

## 📊 Monitoramento e Observabilidade

O projeto utiliza um stack de observabilidade para garantir visibilidade sobre o comportamento dos microserviços:

- **Prometheus**: Coleta de métricas.
- **Grafana**: Painéis para visualização de métricas.
- **OpenTelemetry**: Traces distribuídos.
- **Loki**: Centralização de logs.

---

## 📜 Licença

Este projeto está licenciado sob a licença [MIT](LICENSE.txt).

---

## 📌 Observações

- Este projeto está em constante evolução. Feedbacks e contribuições são bem-vindos!
- Para dúvidas ou problemas, abra uma issue no repositório.

---

## ✨ Contribuições

Contribuições são bem-vindas! Siga os passos abaixo para contribuir:
1. Faça um fork do repositório.
2. Crie uma branch para sua feature ou correção: `git checkout -b minha-feature`.
3. Envie suas alterações: `git push origin minha-feature`.
4. Abra um Pull Request.

---