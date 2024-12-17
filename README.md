# SMSActivate.API

[![](http://img.shields.io/nuget/v/SMSActivate.API.svg)](http://www.nuget.org/packages/SMSActivate.API)

Unofficial SMS-Activate API client

## Content

- [Introduction](#Introduction)

- [Setup](#Setup)

- [Features](#Features)

- [Tips](#Tips)

## Introduction

The library provides an interface for interacting with the SMS Activate API

Built with [public](sms-activate.guru/en/api2), [old JAVA](https://github.com/sms-activate/SMSActivateApi) and decompiled internal (Flutter mobile client) APIs.
For now supports only the SMS activations

## Setup

You can install this package via:

- [NuGet package](http://www.nuget.org/packages/SMSActivate.API) 
- Precompiled binaries from release
- Build a project yourself

Next you need to get or generate the [SMS Activate API token](https://sms-activate.guru/en/profile)

**Notice: You can init API client without token and use some anonymous methods**

And now you are ready to init API client:

```csharp
var saClient = new SAClient(token: "API_TOKEN");
```

## Features

- Get available countries
- Get available mobile operators
- Get activation service list
- Get activation price offers
- Get account balance and cashback information
- Request a phone number for activation
- Get activation status + SMS
- Wait SMS with timeout
- Get active activations
- Set activation status

## Tips

### Metadata

API client caches on RAM the data, which will be processed during work


| Variable name                | Description                            |
|------------------------------|----------------------------------------|
| CachedCountriesInfo          | Available countries info               |
| CachedActivationServicesInfo | Available services for activation      |
| CachedOperatorsInfo          | Available mobile operators info        |
| CachedRentServicesInfo       | Available services for rent            |
| Activations                  | Profile activations (Active + Expired) |
