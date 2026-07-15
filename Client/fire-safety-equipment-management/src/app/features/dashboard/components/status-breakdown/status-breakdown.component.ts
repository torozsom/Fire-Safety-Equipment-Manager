import { Component, Input } from '@angular/core';
import { StatusSummary } from '../../../../core/models/dashboard.models';

/**
 * Status summary enriched with calculated donut-chart positioning.
 */
interface DonutSegment extends StatusSummary {
  /** Fractional start offset for the segment in the donut chart. */
  offset: number;

  /** Fractional share of the total represented by this segment. */
  share: number;
}

/**
 * Status breakdown widget with a donut chart and summary list.
 */
@Component({
  selector: 'app-status-breakdown',
  templateUrl: './status-breakdown.component.html',
  styleUrl: './status-breakdown.component.scss',
})
export class StatusBreakdownComponent {
  /** Status buckets rendered by the chart and list. */
  @Input({ required: true }) items: StatusSummary[] = [];

  /** Total equipment count used to calculate donut segment shares. */
  @Input({ required: true }) total = 0;

  /**
   * Builds donut chart segments from the supplied status buckets.
   *
   * @returns Status rows enriched with cumulative offsets and proportional shares.
   */
  get segments(): DonutSegment[] {
    let offset = 0;

    return this.items.map((item) => {
      const share = this.total > 0 ? item.value / this.total : 0;
      const segment = { ...item, offset, share };
      offset += share;
      return segment;
    });
  }

  /**
   * Formats a numeric percentage for Hungarian dashboard display.
   *
   * @param value Percentage value using a dot decimal separator.
   * @returns Percentage text using a comma decimal separator.
   */
  formatPercentage(value: number): string {
    return `${value.toFixed(1).replace('.', ',')}%`;
  }
}
