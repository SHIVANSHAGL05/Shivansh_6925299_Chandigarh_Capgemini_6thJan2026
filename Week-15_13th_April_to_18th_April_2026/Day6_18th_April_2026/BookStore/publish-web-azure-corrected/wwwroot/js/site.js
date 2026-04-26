const nav = document.querySelector(".shell-nav");

window.addEventListener("scroll", () => {
	if (!nav) {
		return;
	}

	nav.classList.toggle("is-scrolled", window.scrollY > 12);
});

const animatedSections = document.querySelectorAll(".reveal");

if ("IntersectionObserver" in window && animatedSections.length > 0) {
	const observer = new IntersectionObserver((entries, currentObserver) => {
		entries.forEach((entry) => {
			if (!entry.isIntersecting) {
				return;
			}

			entry.target.classList.add("visible");
			currentObserver.unobserve(entry.target);
		});
	},
	{
		threshold: 0.18
	});

	animatedSections.forEach((section, index) => {
		section.style.transitionDelay = `${index * 65}ms`;
		observer.observe(section);
	});
}
// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
