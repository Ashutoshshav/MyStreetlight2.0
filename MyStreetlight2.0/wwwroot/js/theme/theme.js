const html = document.documentElement;
const toggleBtn = document.getElementById("themeToggle");
const icon = toggleBtn.querySelector('i');

// Apply saved theme before page renders
if (localStorage.theme === "dark") {
	// Change to dark mode
	const toggleBtn = document.getElementById("themeToggle");
	const icon = document.getElementById("themeIcon");

	document.documentElement.classList.add("dark");
	icon.classList.remove('fa-moon');
	icon.classList.add('fa-sun');
} else if (localStorage.theme === "light") {
	// Change to light mode
	const toggleBtn = document.getElementById("themeToggle");
	const icon = document.getElementById("themeIcon");

	document.documentElement.classList.remove("dark");
	icon.classList.remove('fa-sun');
	icon.classList.add('fa-moon');
}

toggleBtn.addEventListener("click", () => {
	if (html.classList.contains("dark")) {
		// Change to light mode
		html.classList.remove("dark");
		localStorage.theme = "light";

		icon.classList.remove('fa-sun');
		icon.classList.add('fa-moon');
	} else {
		// Change to dark mode
		html.classList.add("dark");
		localStorage.theme = "dark";

		icon.classList.remove('fa-moon');
		icon.classList.add('fa-sun');
	}
});

//// Theme toggle functionality
//const themeToggle = document.getElementById('theme-toggle');
//const body = document.body;
//const icon = themeToggle.querySelector('i');

//themeToggle.addEventListener('click', () => {
//	body.classList.toggle('dark-mode');

//	if (body.classList.contains('dark-mode')) {
//		// Change to dark mode
//		icon.classList.remove('fa-moon');
//		icon.classList.add('fa-sun');
//		themeToggle.classList.remove('bg-gray-200', 'text-gray-700');
//		themeToggle.classList.add('bg-gray-700', 'text-yellow-300');

//		// Update card backgrounds
//		document.querySelectorAll('.bg-white').forEach(card => {
//			card.classList.remove('bg-white');
//			card.classList.add('dark-card');
//		});
//	} else {
//		// Change to light mode
//		icon.classList.remove('fa-sun');
//		icon.classList.add('fa-moon');
//		themeToggle.classList.remove('bg-gray-700', 'text-yellow-300');
//		themeToggle.classList.add('bg-gray-200', 'text-gray-700');

//		// Update card backgrounds
//		document.querySelectorAll('.dark-card').forEach(card => {
//			card.classList.remove('dark-card');
//			card.classList.add('bg-white');
//		});
//	}
//});