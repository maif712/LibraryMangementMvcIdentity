

document.addEventListener('DOMContentLoaded', () => {
    // Toggle user action tab menu
    const userProfileBtn = document.querySelector(".user-action__user-profile")
    const userActionTab = document.querySelector(".user-action__tabs")
    const userActionLinks = document.querySelectorAll('.user-action__link')

    if (userProfileBtn) {
        userProfileBtn.addEventListener("click", () => {
            const isExpanded = userActionTab.getAttribute("aria-expanded")
            isExpanded == "false" ? userActionTab.setAttribute("aria-expanded", true) : userActionTab.setAttribute("aria-expanded", false)

        })
    }

    // --Close the menu on outside click
    document.addEventListener('click', (event) => {
        if (userActionTab) {
            if (!userActionTab.contains(event.target) && !userProfileBtn.contains(event.target)) {
                userActionTab.setAttribute("aria-expanded", false)
            }
        }
    })

    userActionLinks.forEach(link => {
        link.addEventListener("click", () => {
            userActionTab.setAttribute("aria-expanded", false)
        })
    })
    // End
    
});




