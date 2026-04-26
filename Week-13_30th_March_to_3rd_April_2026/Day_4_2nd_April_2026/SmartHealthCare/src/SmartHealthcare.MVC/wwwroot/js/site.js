document.addEventListener('DOMContentLoaded', function () {
	var revealItems = document.querySelectorAll('[data-reveal]');
	var counterItems = document.querySelectorAll('[data-counter]');
	var tiltItems = document.querySelectorAll('[data-tilt]');

	revealItems.forEach(function (item, index) {
		item.style.transitionDelay = Math.min(index * 55, 360) + 'ms';
	});

	if (!revealItems.length) {
		return;
	}

	if (!('IntersectionObserver' in window)) {
		revealItems.forEach(function (item) {
			item.classList.add('is-visible');
		});
		return;
	}

	var observer = new IntersectionObserver(
		function (entries, io) {
			entries.forEach(function (entry) {
				if (entry.isIntersecting) {
					entry.target.classList.add('is-visible');
					io.unobserve(entry.target);
				}
			});
		},
		{ threshold: 0.15 }
	);

	revealItems.forEach(function (item) {
		observer.observe(item);
	});

	counterItems.forEach(function (counter) {
		var target = Number(counter.getAttribute('data-counter')) || 0;
		var suffix = counter.getAttribute('data-suffix') || '';
		var duration = 900;
		var start = 0;
		var startTime = null;

		function animate(ts) {
			if (!startTime) {
				startTime = ts;
			}

			var progress = Math.min((ts - startTime) / duration, 1);
			var value = Math.floor(start + (target - start) * progress);
			counter.textContent = value + suffix;

			if (progress < 1) {
				window.requestAnimationFrame(animate);
			}
		}

		window.requestAnimationFrame(animate);
	});

	tiltItems.forEach(function (card) {
		card.addEventListener('mousemove', function (event) {
			var rect = card.getBoundingClientRect();
			var x = (event.clientX - rect.left) / rect.width;
			var y = (event.clientY - rect.top) / rect.height;
			var rotateX = (0.5 - y) * 8;
			var rotateY = (x - 0.5) * 8;
			card.style.transform = 'perspective(900px) rotateX(' + rotateX.toFixed(2) + 'deg) rotateY(' + rotateY.toFixed(2) + 'deg)';
		});

		card.addEventListener('mouseleave', function () {
			card.style.transform = 'perspective(900px) rotateX(0deg) rotateY(0deg)';
		});
	});
});
