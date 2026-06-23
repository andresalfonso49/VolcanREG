(function () {
  let chartPromise;
  const charts = {};

  async function chartJs() {
    if (!chartPromise) {
      chartPromise = import("https://cdn.jsdelivr.net/npm/chart.js@4.4.6/+esm");
    }
    return chartPromise;
  }

  function destroy(id) {
    if (charts[id]) {
      charts[id].destroy();
      delete charts[id];
    }
  }

  function normalizeData(data) {
    const source = data ?? {};
    const labels = Object.keys(source);
    const values = Object.values(source);
    if (labels.length === 0) {
      return { labels: ["Sin datos"], values: [0] };
    }
    return { labels, values };
  }

  window.volcanCharts = {
    renderBar: async (canvasId, title, data) => {
      const { Chart } = await chartJs();
      const canvas = document.getElementById(canvasId);
      if (!canvas) return;
      const normalized = normalizeData(data);
      destroy(canvasId);
      charts[canvasId] = new Chart(canvas, {
        type: "bar",
        data: {
          labels: normalized.labels,
          datasets: [{ label: title, data: normalized.values, backgroundColor: "#d6a11f" }]
        },
        options: { responsive: true, plugins: { legend: { display: false }, title: { display: true, text: title } } }
      });
    },

    renderDoughnut: async (canvasId, title, data) => {
      const { Chart } = await chartJs();
      const canvas = document.getElementById(canvasId);
      if (!canvas) return;
      const normalized = normalizeData(data);
      destroy(canvasId);
      charts[canvasId] = new Chart(canvas, {
        type: "doughnut",
        data: {
          labels: normalized.labels,
          datasets: [{ data: normalized.values, backgroundColor: ["#4d78a8", "#2f8f5b", "#d95f3d"] }]
        },
        options: { responsive: true, plugins: { title: { display: true, text: title } } }
      });
    }
  };
})();
