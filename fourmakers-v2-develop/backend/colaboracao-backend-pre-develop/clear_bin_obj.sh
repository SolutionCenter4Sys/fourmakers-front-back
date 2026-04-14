#!/bin/bash

cd ColaboracaoBackend || exit 1

# Lista todas as pastas encontradas
dirs=$(find . -type d \( -name "bin" -o -name "obj" \))

count=0
for dir in $dirs; do
  rm -rf "$dir"
  echo "Removido: $dir"
  count=$((count+1))
done

echo "--------------------------------"
echo "Total de pastas removidas: $count"
