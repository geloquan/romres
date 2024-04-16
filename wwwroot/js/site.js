// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

const canvas = document.getElementById("canvas");
const ctx = canvas.getContext("2d");
const canvasWidth = canvas.width;
const canvasHeight = canvas.height;
const fillColor = "#ff0000"; // Color to fill the area
const tolerance = 10; // Tolerance level for color similarity

// Sample polygon vertices
const polygonVertices = [
    { x: 50, y: 50 },
    { x: 200, y: 100 },
    { x: 300, y: 200 },
    { x: 200, y: 300 },
    { x: 100, y: 200 }
];

// Draw filled polygon on canvas
ctx.beginPath();
ctx.moveTo(polygonVertices[0].x, polygonVertices[0].y);
for (let i = 1; i < polygonVertices.length; i++) {
    ctx.lineTo(polygonVertices[i].x, polygonVertices[i].y);
}
ctx.closePath();
ctx.fillStyle = fillColor;
ctx.fill();

// Get canvas image data
const imageData = ctx.getImageData(0, 0, canvasWidth, canvasHeight);

// Function to perform flood fill
function floodFill(x, y, targetColor) {
    const stack = [{ x: x, y: y }];
    const targetColorRGB = hexToRgb(targetColor);
    const fillColorRGB = hexToRgb(fillColor);

    while (stack.length) {
        const { x, y } = stack.pop();
        const pixelIndex = (y * canvasWidth + x) * 4;
        const currentColor = {
            r: imageData.data[pixelIndex],
            g: imageData.data[pixelIndex + 1],
            b: imageData.data[pixelIndex + 2]
        };

        if (isColorSimilar(currentColor, targetColorRGB, tolerance)) {
            imageData.data[pixelIndex] = fillColorRGB.r;
            imageData.data[pixelIndex + 1] = fillColorRGB.g;
            imageData.data[pixelIndex + 2] = fillColorRGB.b;

            stack.push({ x: x + 1, y: y });
            stack.push({ x: x - 1, y: y });
            stack.push({ x: x, y: y + 1 });
            stack.push({ x: x, y: y - 1 });
        }
    }

    ctx.putImageData(imageData, 0, 0);
}

// Function to check if two colors are similar
function isColorSimilar(color1, color2, tolerance) {
    const deltaR = Math.abs(color1.r - color2.r);
    const deltaG = Math.abs(color1.g - color2.g);
    const deltaB = Math.abs(color1.b - color2.b);
    return deltaR <= tolerance && deltaG <= tolerance && deltaB <= tolerance;
}

// Function to convert hex color to RGB
function hexToRgb(hex) {
    const bigint = parseInt(hex.substring(1), 16);
    return {
        r: (bigint >> 16) & 255,
        g: (bigint >> 8) & 255,
        b: bigint & 255
    };
}