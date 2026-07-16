function stringToColor(str) {
    let hash = 0;
    for (let i = 0; i < str.length; i++) {
        hash = str.charCodeAt(i) + ((hash << 5) - hash);
    }
    let color = '#';
    for (let i = 0; i < 3; i++) {
        let value = (hash >> (i * 8)) & 0xFF;
        // Adjust for darker/more vibrant colors by ensuring a minimum value
        color += ('00' + (value % 150 + 100).toString(16)).substr(-2);
    }
    return color;
}

function initializeUserAvatar() {
    const usernameSpan = document.getElementById('lblUsername');

    const username = usernameSpan ? (usernameSpan.textContent || 'User').trim() : 'User';

    const firstLetter = username.charAt(0).toUpperCase();

    const avatarDiv = document.querySelector('.user-avatar');
    const letterSpan = document.querySelector('.user-avatar .avatar-letter');

    if (avatarDiv && letterSpan) {
        letterSpan.textContent = firstLetter;

        const bgColor = stringToColor(username);

        avatarDiv.style.backgroundColor = bgColor;
    }
}

document.addEventListener('DOMContentLoaded', initializeUserAvatar);