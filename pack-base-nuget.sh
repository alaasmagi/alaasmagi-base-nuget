#!/usr/bin/env bash

set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

projects=(
  "Base.Contracts.Domain/Base.Contracts.Domain.csproj"
  "Base.Contracts.DTO/Base.Contracts.DTO.csproj"
  "Base.Contracts.DataAccess/Base.Contracts.DataAccess.csproj"
  "Base.Contracts.Application/Base.Contracts.Application.csproj"
  "Base.Domain/Base.Domain.csproj"
  "Base.DTO/Base.DTO.csproj"
  "Base.DataAccess.EF/Base.DataAccess.EF.csproj"
  "Base.Application/Base.Application.csproj"
  "Base.Contracts.Exception/Base.Contracts.Exception.csproj"
  "Base.Exception/Base.Exception.csproj"
)

for project in "${projects[@]}"; do
  dotnet pack "$ROOT_DIR/$project" --configuration Release --no-restore
done
