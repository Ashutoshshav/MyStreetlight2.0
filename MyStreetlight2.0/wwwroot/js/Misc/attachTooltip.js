function attachTooltip(element, content) {
    // Create tooltip span
    const tooltip = document.createElement("span");
    tooltip.className = "absolute z-50 -translate-x-1/2 top-full mt-1 " +
        "hidden group-hover:block whitespace-nowrap " +
        "rounded-md bg-black px-3 py-1 text-xs text-white shadow-lg z-50";
    tooltip.innerHTML = content;

    // Ensure element is relatively positioned
    element.classList.add("relative", "group", "inline-block");

    // Append tooltip
    element.appendChild(tooltip);
}
