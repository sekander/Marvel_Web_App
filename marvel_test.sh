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

# Step 4: Make the GET request using curl
#curl -X GET "http://gateway.marvel.com/v1/public/comics?apikey=$apikey&ts=$ts&hash=$hash" \
#curl -X GET "http://gateway.marvel.com/v1/public/stories/32283?apikey=$apikey&ts=$ts&hash=$hash" \
#curl -X GET "http://gateway.marvel.com/v1/public/creators/32/comics?apikey=$apikey&ts=$ts&hash=$hash" \
#curl -X GET "http://gateway.marvel.com/v1/public/comics/16621/?apikey=$apikey&ts=$ts&hash=$hash" \
#curl -X GET "http://gateway.marvel.com/v1/public/characters?name=spider-man&ts=$ts&apikey=$apikey&hash=$hash" \
#curl -X GET "http://gateway.marvel.com/v1/public/characters/1009664/comics?ts=$ts&apikey=$apikey&hash=$hash" \
#
#Gets character id
#curl -X GET "http://gateway.marvel.com/v1/public/characters?ts=$ts&apikey=$apikey&hash=$hash"
#curl -X GET "http://gateway.marvel.com/v1/public/characters?name=Thor&ts=$ts&apikey=$apikey&hash=$hash" \
#curl -X GET "http://gateway.marvel.com/v1/public/characters?name=Loki&ts=$ts&apikey=$apikey&hash=$hash" \
#curl -X GET "http://gateway.marvel.com/v1/public/characters?name=carnage&ts=$ts&apikey=$apikey&hash=$hash" \
#
#curl -X GET "http://gateway.marvel.com/v1/public/characters?name=Spider-Man&ts=$ts&apikey=$apikey&hash=$hash" \
#curl -X GET "http://gateway.marvel.com/v1/public/characters?name=Iron%20Man&ts=$ts&apikey=$apikey&hash=$hash" \
#
#Get List of comics by character id
#curl -X GET "http://gateway.marvel.com/v1/public/characters/1009664/comics?apikey=$apikey&ts=$ts&hash=$hash&format=comic&formatType=comic&limit=30" \
#curl -X GET "http://gateway.marvel.com/v1/public/characters/1009227/comics?apikey=$apikey&ts=$ts&hash=$hash&format=comic&formatType=comic&limit=30" \
#curl -X GET "http://gateway.marvel.com/v1/public/characters/1009368/comics?apikey=$apikey&ts=$ts&hash=$hash&format=comic&formatType=comic&limit=30" \


# Marvel API endpoint (replace with your actual API key, hash, and timestamp)
#API_URL="http://gateway.marvel.com/v1/public/comics"

# Parameters for filtering
#CHARACTER="Spider-Man"
#SERIES="Spider-Man"
#YEAR="2021"
#ARTIST="Mark Bagley"
#ISSUE="1"
#CREATOR="Stan Lee"

# Marvel API call with filters using curl
#curl -s -G "$API_URL" \
#  --data-urlencode "apikey=$apikey" \
#  --data-urlencode "ts=$(date +%s)" \
#  --data-urlencode "hash=$hash" \
#  --data-urlencode "characters=$CHARACTER" \
#  --data-urlencode "series=$SERIES" \
#  --data-urlencode "dateRange=$YEAR-01-01,$YEAR-12-31" \
#  --data-urlencode "issueNumber=$ISSUE" \
#  --data-urlencode "creators=$CREATOR"



# Marvel API endpoint to search characters
#CHARACTER_API_URL="http://gateway.marvel.com/v1/public/characters"
#
## Marvel API call to get characters containing "Spider-Man"
#response=$(curl -s -G "$CHARACTER_API_URL" \
#  --data-urlencode "apikey=$apikey" \
#  --data-urlencode "ts=$ts)" \
#  --data-urlencode "hash=$hash" \
#  --data-urlencode "name=Spider-Man")
#
## Check if the response contains characters and their names
##echo "$response" | jq '.data.results[].name'
#
#
## Check if the response contains any errors
#if echo "$response" | grep -q '"code":409'; then
#  echo "Error: Invalid character or missing valid character. Please check the character name."
#else
#  # If successful, display comic titles without jq
#  #echo "$response" | grep -o '"title":"[^"]*"' | sed 's/"title":"//g'
#  echo "$response" 
#fi
#url="http://gateway.marvel.com/v1/public/characters?ts=$ts&apikey=$apikey&hash=$hash"
#
#
#!/bin/bash

# Define an array of superhero names (escaped for URL safety)
superheroes=(
    "War%20Machine"
    "Green%20Goblin"
    "Ant-Man"
)

counter=0
max_requests=10  # Set max number of requests before breaking
output_file="superheroes.json"

# Initialize the output file as an empty array if it doesn't exist
if [ ! -f "$output_file" ]; then
    echo "[]" > "$output_file"
fi

# Iterate over each superhero name and make a curl request
for superhero in "${superheroes[@]}"; do
    echo "Fetching data for $superhero"
    # Increment the counter
#    ((counter++))

    # If the counter exceeds the max number of requests, break the loop
 #   if [[ $counter -gt $max_requests ]]; then
  #      echo "Reached max request limit of $max_requests. Stopping the script."
   #     break
    #fi
    comicurl="http://gateway.marvel.com/v1/public/characters/$id/comics?apikey=$apikey&ts=$ts&hash=$hash&format=comic&formatType=comic&limit=30" \

    url="http://gateway.marvel.com/v1/public/characters?name=$superhero&ts=$ts&apikey=$apikey&hash=$hash"
    response=$(curl -s "$url")
  id "$id" '{name: $name, id: $id}')
    #curl -s "https://api.example.com/characters/$superhero"

    # Append the new output to the JSON file
    jq ". += [$output]" "$output_file" > temp.json && mv temp.json "$output_file"

    
    echo "$output_file"
done




#url="http://gateway.marvel.com/v1/public/characters?name=Iron%20Man&ts=$ts&apikey=$apikey&hash=$hash"
#response=$(curl -s "$url")
#echo "$response"

#name=$(echo "$response" | jq -r '.data.results[0].name')
#id=$(echo "$response" | jq -r '.data.results[0].id')
# Create a new JSON object with the extracted data
#output=$(jq -n --arg name "$name" --arg id "$id" '{name: $name, id: $id}')

# Print the output
#echo "$output"





