/* Leaflet Map starting from there */

// Update counts
document.getElementById('faultyCount').textContent = faultyLights.length;
document.getElementById('inprocessCount').textContent = inProcessLights.length;
document.getElementById('repairedCount').textContent = repairedLights.length;

// Initialize map
var map = L.map('lightsMap').setView([23.2599, 77.4126], 13);

// Tile layer with better visual appeal
L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
    maxZoom: 19,
    attribution: '&copy; OpenStreetMap contributors'
}).addTo(map);

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
				<div class="popup-header">
					<i class="fas fa-lightbulb text-2xl" style="color: ${color};"></i>
					<div class="flex items-center justify-between">
						<h3 class="font-bold">${light.LightId}</h3>
						<span class="popup-status status-${status}">${status.charAt(0).toUpperCase() + status.slice(1)}</span>
					</div>
				</div>
				<div class="text-sm">
					<p><strong>Address:</strong> ${light.Address || 'N/A'}</p>
				</div>
			</div>
		`;

        marker.bindPopup(popupContent);
        clusterGroup.addLayer(marker);
        allMarkers.push(marker);
    });
}

// Add markers for each status
addMarkers(faultyLights, '#dc2626', 'faulty');
addMarkers(inProcessLights, '#d97706', 'inprocess');
addMarkers(repairedLights, '#059669', 'repaired');

// Fit map to show all markers
if (allMarkers.length > 0) {
    var group = new L.featureGroup(allMarkers);
    map.fitBounds(group.getBounds().pad(0.1));
}

// Filter functionality
document.querySelectorAll('.filter-btn').forEach(btn => {
    btn.addEventListener('click', function () {
        this.classList.toggle('active');
        filterMarkers();
    });
});

function filterMarkers() {
    var activeFilters = [];
    document.querySelectorAll('.filter-btn.active').forEach(btn => {
        activeFilters.push(btn.getAttribute('data-status'));
    });

    clusterGroup.clearLayers();

    allMarkers.forEach(marker => {
        if (activeFilters.includes(marker.status)) {
            clusterGroup.addLayer(marker);
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

    if (searchTerm.length < 2) {
        // Reset to current filter if search term is too short
        filterMarkers();
        return;
    }

    clusterGroup.clearLayers();

    allMarkers.forEach(marker => {
        var light = marker.lightData;
        if (light.LightId.toLowerCase().includes(searchTerm) ||
            (light.Address && light.Address.toLowerCase().includes(searchTerm))) {
            clusterGroup.addLayer(marker);
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

function viewMaintenanceLogs(faultId) {
    fetch(`/Maintenance/ViewLightMaintenanceLogs?faultId=${faultId}`)
        .then(response => response.text())
        .then(html => {
            //console.log(html);

            document.getElementById("maintenanceLogsContainer").innerHTML = html;
            openPopup();
        })
        .catch(err => console.error('Error fetching maintenance logs:', err));
}

function openPopup() {
    document.getElementById('popupModal').classList.remove('hidden');
}

function closePopup() {
    document.getElementById('popupModal').classList.add('hidden');
}


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