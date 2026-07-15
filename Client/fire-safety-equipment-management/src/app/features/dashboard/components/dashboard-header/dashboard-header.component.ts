import { Component, Input } from '@angular/core';
import { IconComponent } from '../../../../shared/icon/icon.component';

/**
 * Header section for dashboard status and reporting period information.
 */
@Component({
  selector: 'app-dashboard-header',
  imports: [IconComponent],
  templateUrl: './dashboard-header.component.html',
  styleUrl: './dashboard-header.component.scss',
})
export class DashboardHeaderComponent {
  /** Short status message describing the current API or fallback state. */
  @Input({ required: true }) apiState = '';

  /** Localized reporting period displayed in the header. */
  @Input({ required: true }) period = '';
}
