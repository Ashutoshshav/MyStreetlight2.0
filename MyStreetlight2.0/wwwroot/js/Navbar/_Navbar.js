const mobileMenu = document.getElementById("mobileMenu");
const mobileMenuButton = document.getElementById("mobileMenuButton");
const closeMobileMenu = document.getElementById("closeMobileMenu");
const overlay = document.getElementById("overlay");

mobileMenuButton.addEventListener("click", () => {
	mobileMenu.classList.remove("-translate-x-full");
	overlay.classList.remove("hidden");
});

closeMobileMenu.addEventListener("click", () => {
	mobileMenu.classList.add("-translate-x-full");
	overlay.classList.add("hidden");
});

overlay.addEventListener("click", () => {
	mobileMenu.classList.add("-translate-x-full");
	overlay.classList.add("hidden");
});