# Imagem base compartilhada: aspnet:8.0 + libgdiplus (pango).
# Usada por todas as APIs/Consumers que precisam de GDI+.
# Rebuild automático no pipeline quando este Dockerfile (ou arquivos aqui) mudam.
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

RUN apt-get update && apt-get install -y --allow-unauthenticated \
    libgif-dev autoconf libtool automake build-essential gettext \
    libglib2.0-dev libcairo2-dev libtiff-dev libexif-dev
RUN apt-get update && apt-get install -y libpango1.0-dev
RUN apt-get install -y --allow-unauthenticated git

RUN git clone https://github.com/mono/libgdiplus.git /libgdiplus
WORKDIR /libgdiplus
RUN ./autogen.sh --with-pango --prefix=/usr
RUN make
RUN make install

WORKDIR /app
