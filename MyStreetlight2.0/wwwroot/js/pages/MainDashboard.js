/* Leaflet Map starting from there */

// Initialize map
var map = L.map('lightsMap').setView([23.2599, 77.4126], 13);

// Tile layer with better visual appeal
L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
    maxZoom: 21.5,
    attribution: '&copy; OpenStreetMap contributors'
})
.addTo(map);

// Create marker clusters
var clusterGroup = L.markerClusterGroup({
    chunkedLoading: true,
    maxClusterRadius: 50,
    spiderfyOnMaxZoom: true,
    showCoverageOnHover: false,
    iconCreateFunction: function (cluster) {
        var count = cluster.getChildCount();
        var size = count < 10 ? 'small' : count < 50 ? 'medium' : 'large';
        return L.divIcon({
            html: '<div class="cluster-marker cluster-' + size + '">' + count + '</div>',
            className: 'marker-cluster-custom',
            iconSize: L.point(40, 40)
        });
    }
});

map.addLayer(clusterGroup);

// Create custom icons with better design
function createLightIcon(color, status) {
    return L.divIcon({
        html: `
			<div class="light-marker" style="background-color: ${color};">
				<i class="fas fa-lightbulb"></i>
			</div>
			<div class="pulse-ring" style="border-color: ${color};"></div>
		`,
        className: `light-marker-${status}`,
        iconSize: [32, 32],
        iconAnchor: [16, 16]
    });
}

// Store all markers for filtering
var allMarkers = [];

// Function to add markers to map
function addMarkers(lights, color, status) {
    lights.forEach(light => {
        var lat = parseFloat(light.Latitude);
        var lng = parseFloat(light.Longitude);

        if (isNaN(lat) || isNaN(lng)) return;

        var marker = L.marker([lat, lng], {
            icon: createLightIcon(color, status)
        });

        // Store status for filtering
        marker.status = status;
        marker.lightData = light;

        // Create popup content
        var popupContent = `
			<div class="custom-popup">
				<div class="popup-header justify-between items-center">
					<div class="flex items-center">
						<i class="fas fa-lightbulb text-lg mr-2" style="color: ${color};"></i>
						<h3 class="font-bold">${light.LightId}</h3>
					</div>
					<div class="flex items-center justify-between">
						<span class="popup-status status-${status}">${status.charAt(0).toUpperCase() + status.slice(1)}</span>
					</div>
				</div>
				<div class="text-sm -my-2">
					<p><strong>Address:</strong>
						<a class="text-blue-800 hover:text-blue-900 hover:underline"
						   href="https://www.google.com/maps?q=${light.Latitude},${light.Longitude}"
						   target="_blank">
						   ${light.Address || 'N/A'}
						</a>
					</p>
				</div>
			</div>
		`;

        marker.bindPopup(popupContent);
		//clusterGroup.addLayer(marker);
		marker.addTo(map);
        allMarkers.push(marker);
    });
}

// Add markers for each status
addMarkers(Lights.filter(light => (light.LightStatus == 1 || light.LightStatus == 0)), '#059669', 'active');
addMarkers(Lights.filter(light => (light.LightStatus == 2)), '#d97706', 'faulty');
addMarkers(Lights.filter(light => (light.LightStatus == 3)), '#dc2626', 'noPower');

// Fit map to show all markers
if (allMarkers.length > 0) {
    var group = new L.featureGroup(allMarkers);
    map.fitBounds(group.getBounds().pad(0.1));
}

// Filter functionality
document.querySelectorAll('.filter-btn').forEach(btn => {
    btn.addEventListener('click', function () {
        this.classList.add('active');
        filterMarkers();
    });
});

function filterMarkers() {
	var activeFilters = [];

	document.querySelectorAll('.filter-btn.active').forEach(btn => {
		activeFilters.push(btn.getAttribute('data-status'));
	});

	// Remove all markers
	allMarkers.forEach(marker => map.removeLayer(marker));

	// Add back filtered markers
	allMarkers.forEach(marker => {
		if (activeFilters.includes(marker.status)) {
			marker.addTo(map);
		}
	});

	// Re-fit map if markers are visible
	var visibleMarkers = allMarkers.filter(m => activeFilters.includes(m.status));
	if (visibleMarkers.length > 0) {
		var group = new L.featureGroup(visibleMarkers);
		map.fitBounds(group.getBounds().pad(0.1));
	}
}

// Search functionality
document.getElementById('searchLights').addEventListener('input', function (e) {
	var searchTerm = e.target.value.toLowerCase();

	// Remove all markers
	allMarkers.forEach(marker => map.removeLayer(marker));

	if (searchTerm.length < 1) {
		// Reset to current filter if search term is too short
		filterMarkers();
		return;
	}

	// Add matching markers
	allMarkers.forEach(marker => {
		var light = marker.lightData;
		if (light.LightId.toLowerCase().includes(searchTerm) ||
			(light.Address && light.Address.toLowerCase().includes(searchTerm))) {
			marker.addTo(map);
		}
	});
});

// Add custom CSS for markers
var style = document.createElement('style');
style.textContent = `
	.light-marker {
		width: 32px;
		height: 32px;
		border-radius: 50%;
		display: flex;
		align-items: center;
		justify-content: center;
		color: white;
		font-size: 14px;
		border: 3px solid white;
		box-shadow: 0 2px 5px rgba(0,0,0,0.2);
		z-index: 10;
		position: relative;
	}

	.pulse-ring {
		position: absolute;
		top: 0;
		left: 0;
		width: 32px;
		height: 32px;
		border-radius: 50%;
		border: 2px solid;
		animation: pulse 2s infinite;
		z-index: 5;
	}

	.light-marker-faulty .pulse-ring { animation-duration: 1.5s; }
	.light-marker-inprocess .pulse-ring { animation-duration: 3s; }
	.light-marker-repaired .pulse-ring { display: none; }

	.marker-cluster-custom {
		background: rgba(255, 255, 255, 0.9);
		border-radius: 50%;
		border: 3px solid;
		font-weight: bold;
		display: flex;
		align-items: center;
		justify-content: center;
		box-shadow: 0 2px 5px rgba(0,0,0,0.2);
	}

	.cluster-marker {
		font-size: 14px;
	}

	.cluster-small { width: 40px; height: 40px; font-size: 12px; border-color: #dc2626; color: #dc2626; }
	.cluster-medium { width: 50px; height: 50px; font-size: 14px; border-color: #d97706; color: #d97706; }
	.cluster-large { width: 60px; height: 60px; font-size: 16px; border-color: #059669; color: #059669; }
`;
document.head.appendChild(style);


// Faults Reported Chart

// console.log(dougnutChartData);
const faultsCtx = document.getElementById('faultsChart').getContext('2d');
const faultsChart = new Chart(faultsCtx, {
	type: 'bar',
	data: {
		labels: ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'],
		datasets: [{
			label: 'Faults Reported',
			data: [320, 450, 380, 510, 290, 180, 220],
			backgroundColor: '#ef4444',
			borderRadius: 6,
			barPercentage: 0.6,
		}]
	},
	options: {
		responsive: true,
		maintainAspectRatio: false,
		plugins: {
			legend: {
				display: false
			}
		},
		scales: {
			y: {
				beginAtZero: true,
				grid: {
					drawBorder: false
				}
			},
			x: {
				grid: {
					display: false
				}
			}
		}
	}
});

// Status Distribution Chart
const statusCtx = document.getElementById('statusChart').getContext('2d');
const statusChart = new Chart(statusCtx, {
	type: 'doughnut',
	data: {
		labels: ['Active', 'Faulty', 'No Power'],
		datasets: [{
			data: [dougnutChartData.ActiveLightCount, dougnutChartData.FaultyLightCount, dougnutChartData.NoPowerLightCount],
			backgroundColor: [
				'#10b981',
				'#ef4444',
				'#f59e0b'
			],
			borderWidth: 0,
			hoverOffset: 12
		}]
	},
	options: {
		responsive: true,
		maintainAspectRatio: false,
		cutout: '70%',
		plugins: {
			legend: {
				position: 'bottom'
			}
		}
	}
});

// Performance Chart
const performanceCtx = document.getElementById('performanceChart').getContext('2d');
const performanceChart = new Chart(performanceCtx, {
	type: 'line',
	data: {
		labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
		datasets: [{
			label: 'Operational Rate (%)',
			data: [97.8, 98.1, 98.3, 98.5, 98.2, 98.6, 98.8, 98.5, 98.7, 98.9, 98.8, 98.5],
			borderColor: '#2563eb',
			backgroundColor: 'rgba(37, 99, 235, 0.1)',
			fill: true,
			tension: 0.3,
			pointRadius: 4,
			pointBackgroundColor: '#2563eb',
			pointBorderColor: '#fff',
			pointBorderWidth: 2
		}]
	},
	options: {
		responsive: true,
		maintainAspectRatio: false,
		scales: {
			y: {
				min: 97,
				max: 100,
				grid: {
					drawBorder: false
				}
			},
			x: {
				grid: {
					display: false
				}
			}
		}
	}
});

//<div class="popup-actions">
//	<div class="action-btn action-view">
//		<i class="fas fa-eye mr-1"></i> View Details
//	</div>
//	${status === 'faulty' ? `
//		<div class="action-btn action-assign">
//			<i class="fas fa-user-plus mr-1"></i> Assign
//		</div>
//	` : ''}
//</div>
