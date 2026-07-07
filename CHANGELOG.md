# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/), and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).


## [1.6.0 - 2026-07-07]
### Fixed
- Fixed thread-safety races in `EnterspeedDeliveryConnection` that could intermittently surface as `NullReferenceException` inside `EnterspeedDeliveryService` under concurrent load, or let a request observe a half-configured `HttpClient`. Each delivery operation now also uses one client capture for both building the request URI and sending the request.

### Changed
- On .NET Core 2.1+/.NET 5+ the `HttpClient` is no longer recreated on a timer. `ConnectionTimeout` now configures `SocketsHttpHandler.PooledConnectionLifetime` (previously hardcoded to 60 seconds, silently ignoring the configured value), which is the supported DNS-rotation mechanism for a long-lived client; `Flush()` still forces a new client on demand. On .NET Framework (the netstandard2.0 asset) timed recreation remains and is now measured with a monotonic clock.
- `ConnectionTimeout` values less than or equal to 0 are treated as 1 second instead of recreating the client on every request.
- `AddEnterspeedDeliveryService` now registers the connection as a singleton (previously transient) and also registers it as `IEnterspeedDeliveryConnection`.
- `EnterspeedDeliveryConnection` now throws `ObjectDisposedException` when used after disposal, and `Dispose()` is idempotent.

## [1.5.0 - 2025-10-01]
### Added
  - Added support for fetching views as strongly typed (.net 6 and up)
  
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