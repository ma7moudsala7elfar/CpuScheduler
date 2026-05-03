// CPU Scheduling Simulator — Visual Enhancements
(function () {
    'use strict';

    const COLORS = [
        '#378ADD', '#1D9E75', '#BA7517',
        '#993556', '#639922', '#D85A30', '#7F77DD'
    ];

    // --- Assign colors to Gantt bars by process index ---
    function colorizeGanttBars() {
        const bars = document.querySelectorAll('.gantt-bar[data-process-index]');
        bars.forEach(function (bar) {
            const idx = parseInt(bar.getAttribute('data-process-index'), 10);
            if (!isNaN(idx)) {
                const color = COLORS[idx % COLORS.length];
                bar.style.backgroundColor = color;
            }
        });

        // Also colorize the process dots in the result table
        const dots = document.querySelectorAll('.process-dot[data-process-index]');
        dots.forEach(function (dot) {
            const idx = parseInt(dot.getAttribute('data-process-index'), 10);
            if (!isNaN(idx)) {
                dot.style.backgroundColor = COLORS[idx % COLORS.length];
            }
        });
    }

    // --- Toggle active class on algorithm selector buttons ---
    function initAlgoToggle() {
        const buttons = document.querySelectorAll('.algo-btn');
        const hiddenInput = document.getElementById('selectedAlgorithm');

        buttons.forEach(function (btn) {
            btn.addEventListener('click', function (e) {
                e.preventDefault();

                buttons.forEach(function (b) { b.classList.remove('active'); });
                btn.classList.add('active');

                if (hiddenInput) {
                    hiddenInput.value = btn.getAttribute('data-algo');
                }
            });
        });
    }

    // --- Initialize on DOM ready ---
    document.addEventListener('DOMContentLoaded', function () {
        colorizeGanttBars();
        initAlgoToggle();
    });
})();
