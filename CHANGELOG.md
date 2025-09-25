# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/), and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).


## [1.4.3 - 2025-05-16]
### Updated
  - Updated dependencies to allow System.Text.Json v9

## [1.4.2 - 2024-06-20]
### Added
- Added response headers to `DeliveryApiResponse` data transfer object

## [1.4.1 - 2024-03-18]
### Added
- Implemented `SocketsHttpHandler` for `NETCOREAPP2_1` and up

### Fixed
- Fixed issue where `X-Api-Key` header was added as deafult header for every request


## [1.4.0 - 2024-02-12]
### Added
- Added support for the POST version of the Delivery API to fetch many ids or handles

### Updated
  - Updated dependencies to allow System.Text.Json v8

## [1.3.0 - 2023-08-10]
### Added
- Added cancellation token support `Fetch` endpoint

## [1.2.0 - 2023-04-20]
### Updated
- Made `enterspeedDeliveryConfiguration` optional in `AddEnterspeedDeliveryService` extension method

## [1.1.0 - 2023-04-18]
### Added
- Added `AddEnterspeedDeliveryService` extension method for easier registration
- Added `WithDeliveryApiUrl` method on `DeliveryQueryBuilder` to handle absolute urls for the delivery API send from webhooks requests

### Updated
  - Updated dependencies to allow System.Text.Json v7

## [1.0.0 - 2022-11-21]
### Breaking
  - Updated default delivery version to 2. Simply set delivery version back to 1 if you are not ready for v2 yet.

### Added
  - Allow configuration of delivery version

## [0.1.3 - 2021-10-16]
### Fixed
  - Fix deserialization for handles without route

## [0.1.2 - 2021-10-16]
### Fixed
  - Fix deserialization for redirects

## [0.1.0 - 2021-11-06]