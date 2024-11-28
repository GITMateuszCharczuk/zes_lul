FROM golang:1.23-alpine as builder

ENV GO111MODULE=on

WORKDIR /app

COPY go.mod ./

RUN go mod tidy

COPY . .

RUN go build -o api-gateway ./cmd/service

FROM alpine:latest

WORKDIR /app

COPY --from=builder /app/api-gateway .

COPY --from=builder /app/swagger-ui ./swagger-ui

EXPOSE 8000

CMD ["./api-gateway"]

