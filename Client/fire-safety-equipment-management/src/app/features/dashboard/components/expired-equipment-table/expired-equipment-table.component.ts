import { Component, Input } from '@angular/core';
import { ExpiredEquipment } from '../../../../core/models/dashboard.models';
import { IconComponent } from '../../../../shared/icon/icon.component';

/**
 * Table widget for equipment with overdue maintenance or inspection dates.
 */
@Component({
  selector: 'app-expired-equipment-table',
  imports: [IconComponent],
  templateUrl: './expired-equipment-table.component.html',
  styleUrl: './expired-equipment-table.component.scss',
})
export class ExpiredEquipmentTableComponent {
  /** Expired equipment rows rendered by the table template. */
  @Input({ required: true }) items: ExpiredEquipment[] = [];
}
