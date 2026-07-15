import { Component, Input } from '@angular/core';
import { ExpiringEquipment } from '../../../../core/models/dashboard.models';
import { IconComponent } from '../../../../shared/icon/icon.component';

/**
 * List widget for equipment that needs attention before an upcoming due date.
 */
@Component({
  selector: 'app-expiring-equipment-list',
  imports: [IconComponent],
  templateUrl: './expiring-equipment-list.component.html',
  styleUrl: './expiring-equipment-list.component.scss',
})
export class ExpiringEquipmentListComponent {
  /** Equipment entries approaching their maintenance or inspection deadline. */
  @Input({ required: true }) items: ExpiringEquipment[] = [];

  /**
   * Derives a display name from the equipment identifier prefix.
   *
   * @param id Inventory identifier such as `TC-...`, `CO2-...`, or `PO-...`.
   * @returns Localized equipment type label used in the list.
   */
  equipmentName(id: string): string {
    if (id.startsWith('TC')) {
      return 'Tűzcsap';
    }

    if (id.startsWith('CO2')) {
      return 'CO2 oltó';
    }

    return 'Porral oltó';
  }
}
