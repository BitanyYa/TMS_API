# API Versioning Policy

## 1. What Counts as a Breaking Change
- Removing a JSON field or property
- Renaming an existing JSON field or property
- Changing HTTP response status codes for existing endpoints
- Tightening request validation rules on an existing API version
- Changing default sort order or pagination envelope structure

## 2. What Counts as Additive (Non-Breaking)
- Adding a new optional response field
- Adding a new API endpoint
- Adding a new optional query parameter to an existing endpoint

## 3. Sunset Window
- Existing API versions (such as V1) are supported for a **6-month minimum** after a new version (V2) is released.

## 4. Communication
- Version deprecation is communicated via HTTP headers:
  - `Deprecation: true`
  - `Sunset: <RFC 7231 Date>`
  - `Link: </api/v2/...>; rel="successor-version"`
- Release notes and CHANGELOG entries published with deprecation notices.

## 5. Version Navigation
- Direct migration across non-sequential versions (e.g., V1 to V3) is supported. Clients are not forced to migrate through intermediate versions.
