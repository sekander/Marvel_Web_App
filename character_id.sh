#!/bin/bash     

# MySQL login credentials
USER="fnky"
PASSWORD="454732"
DATABASE="marvel_api"

json_file=$1
# SQL command to insert data
#SQL="INSERT INTO Characters (id, name) VALUES (1009662, 'Thing');"

# Execute the SQL command
#echo "$SQL" | mycli -u "$USER" -h "192.168.2.87" -p"$PASSWORD" "$DATABASE"

# Loop through each superhero in the JSON file
jq -c '.[]' "$json_file" | while read -r superhero; do
    # Extract the name and id from the JSON object
    name=$(echo "$superhero" | jq -r '.name')
    id=$(echo "$superhero" | jq -r '.id')
    echo ""
    echo "$name : $id"
    SQL="INSERT INTO Characters (id, name) VALUES ($id, '$name');"

    # Execute the SQL command
    #echo "$SQL" | mysql -u "$USER" -p"$PASSWORD" "$DATABASE"
    # Execute the SQL command
    echo "$SQL" | mycli -u "$USER" -h "192.168.2.87" -p"$PASSWORD" "$DATABASE"
    sleep 1
done
