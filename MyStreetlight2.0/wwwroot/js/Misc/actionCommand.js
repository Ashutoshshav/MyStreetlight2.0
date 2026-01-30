function sendCommandForAllLights(actionId) {
    Swal.fire({
        title: 'Enter Password',
        input: 'password',
        inputPlaceholder: 'Enter your password',
        showCancelButton: true,
        confirmButtonText: 'Submit',
        customClass: {
            popup: 'rounded-xl glass-card shadow-lg',
            confirmButton: 'bg-green-600 hover:bg-green-700 font-semibold px-4 py-2 rounded-lg',
            cancelButton: 'bg-red-600 hover:bg-red-700 font-semibold px-4 py-2 rounded-lg'
        },
        preConfirm: (password) => {
            if (!password) Swal.showValidationMessage('Password is required');
            return password;
        }
    }).then((result) => {
        if (result.isConfirmed) {
            // Create a form dynamically and submit it
            const form = document.createElement('form');
            form.method = 'POST';
            form.action = `/Streetlight/SaveActionCommandForAll?actionId=${actionId}`;

            const input = document.createElement('input');
            input.type = 'hidden';
            input.name = 'password';
            input.value = result.value;
            form.appendChild(input);

            document.body.appendChild(form);
            form.submit(); // triggers full POST => redirect => TempData works
        }
    });
}

function sendCommandForGateway(gatewayId, actionId) {
    //console.log(gatewayId, actionId);
    Swal.fire({
        title: 'Enter Password',
        input: 'password',
        inputPlaceholder: 'Enter your password',
        showCancelButton: true,
        confirmButtonText: 'Submit',
        customClass: {
            popup: 'rounded-xl glass-card shadow-lg',
            confirmButton: 'bg-green-600 hover:bg-green-700 font-semibold px-4 py-2 rounded-lg',
            cancelButton: 'bg-red-600 hover:bg-red-700 font-semibold px-4 py-2 rounded-lg'
        },
        preConfirm: (password) => {
            if (!password) Swal.showValidationMessage('Password is required');
            return password;
        }
    }).then((result) => {
        if (result.isConfirmed) {
            // Create a form dynamically and submit it
            const form = document.createElement('form');
            form.method = 'POST';
            form.action = `/Gateway/SaveActionCommandForGateway?gatewayId=${gatewayId}&actionId=${actionId}`;

            const input = document.createElement('input');
            input.type = 'hidden';
            input.name = 'password';
            input.value = result.value;
            form.appendChild(input);

            document.body.appendChild(form);
            form.submit(); // triggers full POST => redirect => TempData works
        }
    });
}

function sendCommandForLight(gatewayId, nodeId, lightId, actionId) {
    Swal.fire({
        title: 'Enter Password',
        input: 'password',
        inputPlaceholder: 'Enter your password',
        showCancelButton: true,
        confirmButtonText: 'Submit',
        customClass: {
            popup: 'rounded-xl glass-card shadow-lg',
            confirmButton: 'bg-green-600 hover:bg-green-700 font-semibold px-4 py-2 rounded-lg',
            cancelButton: 'bg-red-600 hover:bg-red-700 font-semibold px-4 py-2 rounded-lg'
        },
        preConfirm: (password) => {
            if (!password) Swal.showValidationMessage('Password is required');
            return password;
        }
    }).then((result) => {
        if (result.isConfirmed) {
            // Create a form dynamically and submit it
            const form = document.createElement('form');
            form.method = 'POST';
            form.action = `/Streetlight/SaveActionCommand?gatewayId=${gatewayId}&nodeId=${nodeId}&lightId=${lightId}&actionId=${actionId}`;

            const input = document.createElement('input');
            input.type = 'hidden';
            input.name = 'password';
            input.value = result.value;
            form.appendChild(input);

            document.body.appendChild(form);
            form.submit(); // triggers full POST => redirect => TempData works
        }
    });
}
