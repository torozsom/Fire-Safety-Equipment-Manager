/**
 * Supported semantic colors for status summary rows and chart segments.
 */
export type StatusColor = 'green' | 'orange' | 'red' | 'violet';

/**
 * Visual tones available for metric cards.
 */
export type MetricTone = 'danger' | 'warning' | 'success' | 'info';

/**
 * Visual tones available for activity feed entries.
 */
export type ActivityTone = 'success' | 'danger' | 'info' | 'violet';

/**
 * Aggregated equipment status bucket used by the status breakdown widget.
 */
export interface StatusSummary {
  /** Human-readable status label shown in the dashboard. */
  label: string;

  /** Number of equipment records in this status. */
  value: number;

  /** Precomputed percentage value displayed next to the status label. */
  percentage: number;

  /** Semantic color used by the status legend and donut chart. */
  color: StatusColor;
}

/**
 * KPI card data including the current value and historical points for a sparkline.
 */
export interface Metric {
  /** Metric title shown at the top of the card. */
  title: string;

  /** Current numeric value for the metric. */
  value: number;

  /** Supporting text that explains the metric scope or denominator. */
  subtitle: string;

  /** Semantic tone used to style the metric card. */
  tone: MetricTone;

  /** Ordered data points rendered as the card sparkline. */
  trend: number[];
}

/**
 * Equipment item with an upcoming inspection or maintenance deadline.
 */
export interface ExpiringEquipment {
  /** Equipment inventory identifier. */
  id: string;

  /** Site or room where the equipment is installed. */
  location: string;

  /** Number of days remaining until the deadline. */
  daysLeft: number;
}

/**
 * Equipment item whose inspection or maintenance deadline has already passed.
 */
export interface ExpiredEquipment {
  /** Equipment inventory identifier. */
  id: string;

  /** Equipment category displayed in the expired equipment table. */
  type: string;

  /** Site or area where the expired equipment is installed. */
  site: string;

  /** Localized due date text shown to the user. */
  dueDate: string;

  /** Localized status label for the table row. */
  status: string;
}

/**
 * Recent dashboard activity entry.
 */
export interface Activity {
  /** Action title shown in the activity feed. */
  title: string;

  /** Equipment, document, or workflow identifier affected by the activity. */
  target: string;

  /** Localized relative timestamp displayed in the feed. */
  time: string;

  /** Semantic tone used to select the activity icon and accent color. */
  tone: ActivityTone;
}

/**
 * Complete dashboard view model returned by the API and consumed by the dashboard page.
 */
export interface DashboardData {
  /** Message describing whether data came from the API or the local mock fallback. */
  healthMessage: string;

  /** Localized reporting period shown in the dashboard header. */
  period: string;

  /** KPI cards rendered at the top of the dashboard. */
  metrics: Metric[];

  /** Status buckets used by the status breakdown visualization. */
  statusSummary: StatusSummary[];

  /** Equipment approaching a due date. */
  expiringEquipment: ExpiringEquipment[];

  /** Equipment already past due. */
  expiredEquipment: ExpiredEquipment[];

  /** Recent operational events shown in the activity feed. */
  activities: Activity[];
}
