#!/bin/bash

# @param database:string required
# @param outputDir:string optional default="/var/backups"
# @param port:int optional default=5432
# @param compress:bool optional default=true
# @param retentionDays:int optional default=7

set -euo pipefail

# Positional arguments, read in declaration order. A value is always present at
# each index because C# passes the @param default (or "") for optional args.
database="$1"
output_dir="$2"
port="$3"
compress="$4"
retention_days="$5"

timestamp="$(date +%Y%m%d_%H%M%S)"
backup_file="${output_dir}/${database}_${timestamp}.sql"

echo "Backing up database '${database}' (port ${port})"
echo "Target: ${backup_file}"

if [ "$compress" = "true" ]; then
  backup_file="${backup_file}.gz"
  echo "Compression enabled -> ${backup_file}"
fi

echo "Pruning backups older than ${retention_days} day(s) in ${output_dir}"
echo "Backup complete: ${backup_file}"
