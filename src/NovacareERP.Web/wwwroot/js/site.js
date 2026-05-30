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

document.querySelectorAll("[data-modal-open]").forEach((trigger) => {
  trigger.addEventListener("click", () => {
    const modal = document.getElementById(trigger.getAttribute("data-modal-open"));
    modal?.classList.add("is-open");
  });
});

document.querySelectorAll("[data-modal-close]").forEach((trigger) => {
  trigger.addEventListener("click", () => {
    trigger.closest(".modal-preview")?.classList.remove("is-open");
  });
});

document.querySelectorAll(".modal-preview").forEach((modal) => {
  modal.addEventListener("click", (event) => {
    if (event.target === modal) {
      modal.classList.remove("is-open");
    }
  });
});

const showDemoNotice = (message) => {
  const existingNotice = document.querySelector(".demo-notice");
  existingNotice?.remove();

  const notice = document.createElement("div");
  notice.className = "demo-notice";
  notice.textContent = message;
  document.body.appendChild(notice);

  window.setTimeout(() => {
    notice.classList.add("is-visible");
  }, 10);

  window.setTimeout(() => {
    notice.classList.remove("is-visible");
    window.setTimeout(() => notice.remove(), 240);
  }, 2600);
};

document.querySelectorAll("[data-demo-submit]").forEach((button) => {
  button.addEventListener("click", () => {
    showDemoNotice(button.getAttribute("data-demo-submit") || "Islem hazirlandi.");
    button.closest(".modal-preview")?.classList.remove("is-open");
  });
});

document.querySelectorAll("[data-production-select]").forEach((select) => {
  select.addEventListener("change", () => {
    const panel = select.closest(".production-panel");
    const compose = panel?.querySelector("[data-production-compose]");
    const steps = document.querySelectorAll("[data-production-steps] span");
    const hasProduct = select.selectedIndex > 0;

    if (compose) {
      compose.hidden = !hasProduct;
    }

    steps.forEach((step, index) => {
      step.classList.toggle("active", hasProduct ? index <= 1 : index === 0);
    });
  });
});

document.querySelectorAll("[data-production-save]").forEach((button) => {
  button.addEventListener("click", () => {
    document.querySelectorAll("[data-production-steps] span").forEach((step) => {
      step.classList.add("active");
    });
    showDemoNotice("Uretim recetesi taslak olarak hazirlandi.");
  });
});

document.querySelectorAll("[data-settings-tab-target]").forEach((tabButton) => {
  tabButton.addEventListener("click", () => {
    const target = tabButton.getAttribute("data-settings-tab-target");
    const card = tabButton.closest(".settings-editor-card");

    card?.querySelectorAll("[data-settings-tab-target]").forEach((button) => {
      button.classList.toggle("active", button === tabButton);
    });

    card?.querySelectorAll("[data-settings-tab-panel]").forEach((panel) => {
      panel.classList.toggle("active", panel.getAttribute("data-settings-tab-panel") === target);
    });
  });
});
