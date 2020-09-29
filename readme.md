De ARWeather applicatie bestaat uit 2 delen. Een server die API calls kan maken naar Open Weather Map, en de vergaarde data in een database cached. En een Unity Android App die een weer visualisatie toont in AR met behulp van data van het eerder benoemde server component.

De server draait in NodeJS met een lokale MySQL database. Data van Open Weather Map worden opgeslagen in deze database en gebruikt voor toekomstige calls totdat deze data outdated is.

De app is een Unity app gemaakt met AR Foundation die de location service gebruikt om de lokale weersvoorspellingen op te halen

De server is live voor het testen van de API via het volgende address:
https://ec2-3-15-240-86.us-east-2.compute.amazonaws.com:3000/weather/51.590432/4.761887

Screenshot van de applicatie:
![App Screenshot](https://i.imgur.com/o5KbbVx.jpg)
