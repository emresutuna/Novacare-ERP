// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

document.querySelectorAll("[data-tab-target]").forEach((tabButton) => {
  tabButton.addEventListener("click", () => {
    const target = tabButton.getAttribute("data-tab-target");
    const form = tabButton.closest(".customer-create");

    form.querySelectorAll("[data-tab-target]").forEach((button) => {
      button.classList.toggle("active", button === tabButton);
    });

    form.querySelectorAll("[data-tab-panel]").forEach((panel) => {
      panel.classList.toggle("active", panel.getAttribute("data-tab-panel") === target);
    });
  });
});
