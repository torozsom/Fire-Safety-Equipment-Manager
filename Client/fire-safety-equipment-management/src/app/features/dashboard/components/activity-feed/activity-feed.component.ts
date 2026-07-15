import { Component, Input } from '@angular/core';
import { Activity, ActivityTone } from '../../../../core/models/dashboard.models';
import { IconComponent, IconName } from '../../../../shared/icon/icon.component';

/**
 * Presentational component that renders the latest dashboard activity entries.
 */
@Component({
  selector: 'app-activity-feed',
  imports: [IconComponent],
  templateUrl: './activity-feed.component.html',
  styleUrl: './activity-feed.component.scss',
})
export class ActivityFeedComponent {
  /** Activity entries supplied by the dashboard page. */
  @Input({ required: true }) activities: Activity[] = [];

  /**
   * Selects the icon that matches an activity tone.
   *
   * @param tone Semantic activity tone from the dashboard view model.
   * @returns Icon name consumed by the shared icon component.
   */
  iconFor(tone: ActivityTone): IconName {
    const icons: Record<ActivityTone, IconName> = {
      success: 'calendar',
      danger: 'alert',
      info: 'edit',
      violet: 'file',
    };

    return icons[tone];
  }
}
