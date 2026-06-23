(function () {
  const palette = {
    primary: "#2563EB",
    primaryDark: "#1E3A8A",
    primarySoft: "#DBEAFE",
    success: "#059669",
    successSoft: "#D1FAE5",
    danger: "#DC2626",
    warning: "#D97706",
    border: "#E2E8F0",
    text: "#0F172A",
    muted: "#64748B"
  };

  function normalizeData(data) {
    const source = data ?? {};
    const labels = Object.keys(source);
    const values = Object.values(source).map(value => Number(value ?? 0));
    if (labels.length === 0) {
      return { labels: ["Sin datos"], values: [0] };
    }
    return { labels, values };
  }

  function setup(canvas) {
    const rect = canvas.getBoundingClientRect();
    const parentWidth = canvas.parentElement?.clientWidth || 0;
    const availableWidth = Math.floor(parentWidth || rect.width || 320);
    const width = Math.max(240, availableWidth);
    const height = Math.max(260, Math.floor(rect.height || 300));
    const ratio = window.devicePixelRatio || 1;
    canvas.width = width * ratio;
    canvas.height = height * ratio;
    canvas.style.width = `${width}px`;
    canvas.style.height = `${height}px`;

    const ctx = canvas.getContext("2d");
    ctx.setTransform(ratio, 0, 0, ratio, 0, 0);
    ctx.clearRect(0, 0, width, height);
    ctx.lineWidth = 2;
    return { ctx, width, height };
  }

  function title(ctx, value) {
    ctx.fillStyle = palette.text;
    ctx.font = "800 15px Segoe UI, sans-serif";
    ctx.fillText(value, 18, 26);
  }

  function empty(ctx, width, height) {
    ctx.fillStyle = palette.muted;
    ctx.font = "600 14px Segoe UI, sans-serif";
    ctx.fillText("Sin datos", width / 2 - 28, height / 2);
  }

  function format(value) {
    return Number.isInteger(value) ? value.toString() : value.toFixed(2);
  }

  function drawAxes(ctx, left, top, plotWidth, plotHeight) {
    ctx.strokeStyle = palette.border;
    ctx.beginPath();
    ctx.moveTo(left, top);
    ctx.lineTo(left, top + plotHeight);
    ctx.lineTo(left + plotWidth, top + plotHeight);
    ctx.stroke();

    ctx.strokeStyle = "#F1F5F9";
    ctx.lineWidth = 1;
    for (let i = 1; i <= 3; i += 1) {
      const y = top + (plotHeight / 4) * i;
      ctx.beginPath();
      ctx.moveTo(left, y);
      ctx.lineTo(left + plotWidth, y);
      ctx.stroke();
    }
    ctx.lineWidth = 2;
  }

  function drawLabel(ctx, label, x, y) {
    const text = label.length > 10 ? `${label.slice(0, 9)}...` : label;
    ctx.save();
    ctx.translate(x, y);
    ctx.fillStyle = palette.muted;
    ctx.font = "600 11px Segoe UI, sans-serif";
    ctx.textAlign = "center";
    ctx.fillText(text, 0, 0);
    ctx.restore();
  }

  function clampTextX(ctx, text, x, width, padding = 10) {
    const textWidth = ctx.measureText(text).width;
    return Math.min(Math.max(x, padding), width - textWidth - padding);
  }

  function shouldDrawLabel(index, count) {
    if (count <= 8) return true;
    const step = Math.ceil(count / 7);
    return index % step === 0 || index === count - 1;
  }

  function colorForLabel(label, index) {
    const normalized = label.toLowerCase();
    if (normalized.includes("validated") || normalized.includes("validado")) return palette.success;
    if (normalized.includes("incorrect")) return palette.danger;
    if (normalized.includes("notvalidated") || normalized.includes("pendiente")) return palette.primary;
    const colors = [palette.primary, "#3B82F6", palette.primaryDark, "#0EA5E9", "#6366F1"];
    return colors[index % colors.length];
  }

  function renderBar(canvasId, chartTitle, data) {
    const canvas = document.getElementById(canvasId);
    if (!canvas) return;

    const normalized = normalizeData(data);
    const { ctx, width, height } = setup(canvas);
    const labels = normalized.labels;
    const values = normalized.values;
    const max = Math.max(...values, 0);
    title(ctx, chartTitle);

    if (max === 0) {
      empty(ctx, width, height);
      return;
    }

    const left = 48;
    const right = 18;
    const top = 48;
    const bottom = 52;
    const plotWidth = width - left - right;
    const plotHeight = height - top - bottom;
    drawAxes(ctx, left, top, plotWidth, plotHeight);

    const gap = Math.max(8, plotWidth / labels.length * 0.18);
    const barWidth = Math.max(12, (plotWidth - gap * (labels.length + 1)) / labels.length);

    labels.forEach((label, index) => {
      const value = values[index];
      const x = left + gap + index * (barWidth + gap);
      const h = value / max * plotHeight;
      const y = top + plotHeight - h;

      ctx.fillStyle = colorForLabel(label, index);
      ctx.fillRect(x, y, barWidth, h);
      ctx.fillStyle = palette.text;
      ctx.font = "700 12px Segoe UI, sans-serif";
      const valueText = format(value);
      ctx.fillText(valueText, clampTextX(ctx, valueText, x, width), Math.max(top + 12, y - 6));
      if (shouldDrawLabel(index, labels.length)) {
        drawLabel(ctx, label, x + barWidth / 2, top + plotHeight + 22);
      }
    });
  }

  function renderLine(canvasId, chartTitle, data) {
    const canvas = document.getElementById(canvasId);
    if (!canvas) return;

    const normalized = normalizeData(data);
    const { ctx, width, height } = setup(canvas);
    const labels = normalized.labels;
    const values = normalized.values;
    const max = Math.max(...values, 0);
    title(ctx, chartTitle);

    if (max === 0) {
      empty(ctx, width, height);
      return;
    }

    const left = 48;
    const right = 18;
    const top = 48;
    const bottom = 52;
    const plotWidth = width - left - right;
    const plotHeight = height - top - bottom;
    drawAxes(ctx, left, top, plotWidth, plotHeight);

    const points = values.map((value, index) => ({
      x: labels.length === 1 ? left + plotWidth / 2 : left + index * (plotWidth / (labels.length - 1)),
      y: top + plotHeight - value / max * plotHeight,
      value,
      label: labels[index]
    }));

    ctx.strokeStyle = palette.primary;
    ctx.lineWidth = 3;
    ctx.beginPath();
    points.forEach((point, index) => {
      if (index === 0) ctx.moveTo(point.x, point.y);
      else ctx.lineTo(point.x, point.y);
    });
    ctx.stroke();

    points.forEach((point, index) => {
      ctx.fillStyle = palette.primary;
      ctx.beginPath();
      ctx.arc(point.x, point.y, 4, 0, Math.PI * 2);
      ctx.fill();
      ctx.fillStyle = palette.text;
      ctx.font = "700 12px Segoe UI, sans-serif";
      const valueText = format(point.value);
      ctx.fillText(valueText, clampTextX(ctx, valueText, point.x - 10, width), Math.max(top + 12, point.y - 9));
      if (shouldDrawLabel(index, points.length)) {
        drawLabel(ctx, point.label, point.x, top + plotHeight + 22);
      }
    });
  }

  window.volcanCharts = {
    renderBar,
    renderLine,
    renderDoughnut: renderBar
  };
})();
