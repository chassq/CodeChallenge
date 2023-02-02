# Code Challenge Project

## To Get The Project Working

## Dependencies: 
1. .NET 7
1. VS 2022
1. This application uses the Twitter v2 API. See: https://developer.twitter.com/en/docs/twitter-api/getting-started/about-twitter-api

## Authentication

1. Right click on the CodeChallengeAPI project and select Manage User Secrets
2. In the secrets.json file place the json below. You will need to get the specified values from an application you registered in the Twitter developer portal.
    see: https://developer.twitter.com/

```json
{
    "TwitterAuthConfig": {
        "ApplicationName": "YOUR TWITTER APP NAME HERE",
        "ConsumerKey": "YOUR TWITTER CONSUMER KEY HERE",
        "ConsumerSecret": "YOUR TWITTER CONSUMER SECRET HERE"
    }
}
```

## Start The Project
1. Make sure the CodeChallengeAPI project is set as the startup project and start the project with or without debugging.