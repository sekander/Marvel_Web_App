#!/bin/bash

# Set your Marvel API public and private keys
public_key="94d724da40747c7c24a05fd75d735efb"
private_key="cd6eeac5e4a7e5aef4ae01faf0bef3b46d1834de"

# Step 1: Generate a timestamp
ts=$(date +%s)

# Step 2: Generate the hash by concatenating ts, private key, and public key, then hashing with MD5
hash=$(echo -n "$ts$private_key$public_key" | md5sum | awk '{print $1}')

# Step 3: Set the API key
apikey="$public_key"

# Path to the superheroes.json file
#json_file="superheroes.json"
json_file=$1

# MySQL login credentials
USER="fnky"
PASSWORD="454732"
DATABASE="marvel_api"

counter=0
max_requests=5  # Set max number of requests before breaking

# Loop through each superhero in the JSON file
jq -c '.[]' "$json_file" | while read -r superhero; do
   # ((counter++))

    # If the counter exceeds the max number of requests, break the loop
    #if [[ $counter -gt $max_requests ]]; then
    #    echo "Reached max request limit of $max_requests. Stopping the script."
    #    break
    #fi
     
    # Extract the name and id from the JSON object
    name=$(echo "$superhero" | jq -r '.name')
    character_id=$(echo "$superhero" | jq -r '.id')
    echo ""
    echo "$name : $character_id"


    comicurl="http://gateway.marvel.com/v1/public/characters/$character_id/comics?apikey=$apikey&ts=$ts&hash=$hash&format=comic&formatType=comic&limit=10" \
    #comicurl="http://gateway.marvel.com/v1/public/characters/$id/comics?apikey=$apikey&ts=$ts&hash=$hash&format=comic&formatType=comic&limit=1" \
    #comicurl="http://gateway.marvel.com/v1/public/characters/$id/comics?apikey=$apikey&ts=$ts&hash=$hash&format=comic&formatType=comic&limit=4" \
    response=$(curl -s "$comicurl")

    # Get the number of results
    length=$(echo "$response" | jq '.data.results | length')
    echo "$length"
    echo "=============================="
    echo ""

    #echo "$response"
    # Loop through all results
    for ((i=0; i<length; i++)); do

        echo ""
        echo ""
        echo "Comic $i"
        # Extracting important data
        #name=$(echo "$response" | jq -r '.data.results[0].name')
        id=$(echo "$response" | jq -r ".data.results[$i].id")
        image_url=$(echo "$response" | jq -r ".data.results[$i].images[0].path")
        title=$(echo "$response" | jq -r ".data.results[$i].title")
        description=$(echo "$response" | jq -r ".data.results[$i].description")
        description=$(echo "$description" | sed "s/'/''/g")
        onsale_date=$(echo "$response" | jq -r ".data.results[$i].dates[0].date")
        price=$(echo "$response" | jq -r ".data.results[$i].prices[0].price")
        thumbnail=$(echo "$response" | jq -r ".data.results[$i].thumbnail.path")
        details_url=$(echo "$response" | jq -r ".data.results[$i].urls[0].url")
        issueNumber=$(echo "$response" | jq -r ".data.results[$i].issueNumber")
        pageCount=$(echo "$response" | jq -r ".data.results[$i].pageCount")
        series=$(echo "$response" | jq -r ".data.results[$i].series.name")

        releaseDate=$(echo "$response" | jq -r ".data.results[$i].dates[]? | select(.type==\"onsaleDate\") | .date // \"N/A\"")

        # Extract writer(s) safely
        writer=$(echo "$response" | jq -r ".data.results[$i].creators.items[]? | select(.role == \"writer\") | .name" | paste -sd ", ")

        # Extract artist(s) (inker, penciler, colorist)
        artists=$(echo "$response" | jq -r ".data.results[$i].creators.items[]? | select(.role == \"inker\" or .role == \"penciler\" or .role == \"colorist\") | .name" | paste -sd ", ")

        # Extract character names
        characters=$(echo "$response" | jq -r ".data.results[$i].characters.items[].name")

        # Displaying the extracted data
        echo "Title: $title"
        echo "Issue Number: $issueNumber"
        echo "Series: $series"
        echo "Description: $description"
        echo "Page Count: $pageCount"
            echo "Release Date: $releaseDate"
        echo "On Sale Date: $onsale_date"
        echo "Writer(s): $writer"
        echo "Artist(s): $artists"
        echo "Price: $price"
        echo "Thumbnail: $thumbnail"
        echo "Details URL: $details_url"
        echo "Characters: $characters"


        
        # Convert the multi-line string to a comma-separated JSON array
characters_json=$(echo "$characters" | tr '\n' ',' | sed 's/,$//')  # Replace newlines with commas, remove trailing comma
characters_json="[$characters_json]"  # Add square brackets around the list

# Output the resulting JSON array
echo "$characters_json"



        #echo "Name: $name"
        echo "ID: $id"
        echo "Image URL: $image_url"


        SQL="INSERT INTO Comics (id, 
                         title, 
                         issue_number, 
                         series, 
                         description, 
                         page_count, 
                         release_date, 
                         on_sale_date, 
                         writer, 
                         artist, 
                         price, 
                         thumbnail_url, 
                         details_url, 
                         character_id, 
                         image_url, 
                         characters) 
      VALUES ($id, 
              '$title',
              $issueNumber,
              '$series',
              '$description',
              $pageCount,
              '$releaseDate',
              '$on_sale_date',
              '$writer',
              '$artist',
              $price,
              '$thumbnail',
              '$details_url',
              $character_id,
              '$image_url',
              '$characters_json');"



        echo "$SQL" | mycli -u "$USER" -h "192.168.2.87" -p"$PASSWORD" "$DATABASE"
        echo "Inserted Comics"

        echo "------------------------------"
        sleep 1
    done
done

