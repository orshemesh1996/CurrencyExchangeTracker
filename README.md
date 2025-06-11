# Currency Exchange Tracker

A system for tracking real-time currency exchange rates.

## Features
- Fetches real-time exchange rate data every 10 seconds
- Provides an API for accessing the latest rates
- Stores data in a local SQLite database
- Includes a Swagger interface for easy testing

## Running the Project
To run the solution correctly, you must start **both services**:

1. Open the solution in Visual Studio
2. Go to **Solution Properties → Startup Project**
3. Choose **Multiple startup projects**
4. Set the following projects to **Start**:
   - `CurrencyExchangeTracker.RateCollector` – responsible for collecting and updating exchange rates
   - `CurrencyExchangeTracker.Api` – exposes the HTTP API endpoints
5. Press **F5** to run both projects together

## API Endpoints

- `GET /api/currencyrates`  
  Returns all available exchange rates.

- `GET /api/currencyrates/{pair}`  
  Returns the rate for a specific currency pair (e.g., `USD-ILS`, `EUR-ILS`, etc.)

- `GET /api/currencyrates/status`  
  Returns the current status of the background service (e.g., last update time).

## Example Usage

Open your browser or Postman and navigate to:  
`https://localhost:7021/swagger/index.html`

From there, you can test all the endpoints interactively.

---
