// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

export function loadHtml(targetId, filePath) {
    fetch(filePath)
        .then(response => {
            if (!response.ok) {
                throw new Error(`Failed to load ${filePath}: ${response.statusText}`);
            }
            return response.text();
        })
        .then(html => {
            document.getElementById(targetId).innerHTML = html;

            // If loading the nav, set the CSRF token value (if needed)
            if (filePath.includes('nav.html')) {
                const token = document.querySelector('[name="__RequestVerificationToken"]')?.value;
                if (token) {
                    document.querySelector('#' + targetId + ' input[name="__RequestVerificationToken"]').value = token;
                }
            }
        })
        .catch(error => console.error('Error loading HTML:', error));
}
