import { Component, Input } from '@angular/core';
import { Metric, MetricTone } from '../../../../core/models/dashboard.models';

/**
 * Responsive grid of dashboard KPI cards.
 *
 * Each card uses the metric tone for visual emphasis and renders its trend values as an
 * inline SVG sparkline.
 */
@Component({
  selector: 'app-metrics-grid',
  templateUrl: './metrics-grid.component.html',
  styleUrl: './metrics-grid.component.scss',
})
export class MetricsGridComponent {
  /** Metric cards supplied by the dashboard page. */
  @Input({ required: true }) metrics: Metric[] = [];

  /**
   * Selects the compact glyph shown for a metric tone.
   *
   * @param tone Semantic tone assigned to the metric card.
   * @returns One-character symbol rendered by the card template.
   */
  metricIcon(tone: MetricTone): string {
    const icons: Record<MetricTone, string> = {
      danger: '!',
      warning: '◷',
      success: '✓',
      info: '⌁',
    };

    return icons[tone];
  }

  /**
   * Converts ordered trend values into an SVG path string.
   *
   * Values are normalized to the fixed card sparkline viewport so the template can bind
   * the returned value directly to the `d` attribute.
   *
   * @param points Ordered metric trend values.
   * @returns SVG path data for the sparkline polyline.
   */
  sparklinePath(points: number[]): string {
    const width = 106;
    const height = 34;
    const min = Math.min(...points);
    const max = Math.max(...points);
    const range = Math.max(max - min, 1);

    return points
      .map((point, index) => {
        const x = (index / (points.length - 1)) * width;
        const y = height - ((point - min) / range) * (height - 7) - 3;
        return `${index === 0 ? 'M' : 'L'} ${x.toFixed(1)} ${y.toFixed(1)}`;
      })
      .join(' ');
  }
}
