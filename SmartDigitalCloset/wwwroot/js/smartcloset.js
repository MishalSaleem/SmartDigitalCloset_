window.SmartCloset = {
    getUserEmail: function() {
        return localStorage.getItem('userEmail') || '';
    },
    setUserEmail: function(email) {
        localStorage.setItem('userEmail', email);
    },
    getSavedInterests: function() {
        const data = localStorage.getItem('savedInterests');
        try {
            return data ? JSON.parse(data) : [];
        } catch {
            return [];
        }
    },
    getJoinedDate: function() {
        return localStorage.getItem('joinedDate') || '';
    },
    setJoinedDate: function(date) {
        localStorage.setItem('joinedDate', date);
    },
    saveInterest: function(interest) {
        let interests = SmartCloset.getSavedInterests();
        if (!interests.includes(interest)) {
            interests.push(interest);
            localStorage.setItem('savedInterests', JSON.stringify(interests));
        }
    },
    removeInterest: function(interest) {
        let interests = SmartCloset.getSavedInterests();
        interests = interests.filter(i => i !== interest);
        localStorage.setItem('savedInterests', JSON.stringify(interests));
    },
    clearInterests: function() {
        localStorage.removeItem('savedInterests');
    }
};

// Add functions for profile photo handling
window.clickElement = function(element) {
    element.click();
};

window.getFileAsBase64 = function(inputElement) {
    return new Promise((resolve, reject) => {
        const fileInput = inputElement;
        const file = fileInput.files[0];
        
        if (!file) {
            reject("No file selected");
            return;
        }

        const reader = new FileReader();
        reader.readAsDataURL(file);
        
        reader.onload = function() {
            resolve(reader.result);
        };
        
        reader.onerror = function(error) {
            reject('Error reading file: ' + error);
        };
    });
};
