/**
 * @jest-environment jsdom
 */
const fetch = require('node-fetch');

// Mock the global fetch to control its behavior
global.fetch = jest.fn(() =>
    Promise.resolve({
        ok: true,
        json: () => Promise.resolve([
            { name: "Crafted Necklace", price: 150 },
            { name: "Artisan Vase", price: 90 }
        ]),
    })
);

// Import your functions from the script file
// You might need to export them in your actual `scripts.js` or adjust for testing
const { fetchItems, displayResults } = require('./PriceRangeSorting');

describe('fetchItems function', () => {
    beforeEach(() => {
        fetch.mockClear();
    });

    it('fetches items successfully from an API', async () => {
        document.body.innerHTML = `
      <input id="bottomPrice" value="50">
      <input id="topPrice" value="200">
      <div id="results"></div>
      <div id="loading"></div>
    `;

        await fetchItems();
        expect(fetch).toHaveBeenCalledTimes(1);
        expect(fetch).toHaveBeenCalledWith("http://localhost:8080/Item/api/sort?bottomPrice=50&topPrice=200");
    });

    it('handles minimum price greater than maximum price', () => {
        document.body.innerHTML = `
      <input id="bottomPrice" value="250">
      <input id="topPrice" value="200">
      <div id="results"></div>
      <div id="loading"></div>
    `;

        const alertMock = jest.spyOn(window, 'alert').mockImplementation(() => { });
        fetchItems();
        expect(alertMock).toHaveBeenCalledWith("Maximum price should be greater than minimum price.");
        alertMock.mockRestore();
    });
});

describe('displayResults function', () => {
    it('displays items correctly', () => {
        document.body.innerHTML = '<div id="results"></div>';
        const resultsDiv = document.getElementById('results');

        displayResults([
            { name: "Crafted Necklace", price: 150 },
            { name: "Artisan Vase", price: 90 }
        ]);

        expect(resultsDiv.innerHTML.includes("Crafted Necklace - $150.00")).toBe(true);
        expect(resultsDiv.innerHTML.includes("Artisan Vase - $90.00")).toBe(true);
    });

    it('displays no items found message when empty array is passed', () => {
        document.body.innerHTML = '<div id="results"></div>';
        const resultsDiv = document.getElementById('results');

        displayResults([]);

        expect(resultsDiv.textContent).toMatch("No items found within the specified price range.");
    });
});