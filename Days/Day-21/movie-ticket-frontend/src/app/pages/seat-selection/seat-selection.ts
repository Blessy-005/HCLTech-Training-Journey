import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-seat-selection',
  imports: [RouterLink],
  templateUrl: './seat-selection.html',
  styleUrl: './seat-selection.css'
})
export class SeatSelection {
  rows = ['A', 'B', 'C', 'D', 'E'];
  seatNumbers = [1, 2, 3, 4, 5, 6, 7, 8];

  bookedSeats = ['A3', 'A4', 'B6', 'C2', 'D7'];
  selectedSeats: string[] = [];

  ticketPrice = 180;

  toggleSeat(row: string, number: number): void {
    const seat = row + number;

    if (this.bookedSeats.includes(seat)) {
      return;
    }

    if (this.selectedSeats.includes(seat)) {
      this.selectedSeats = this.selectedSeats.filter(s => s !== seat);
    } else {
      this.selectedSeats.push(seat);
    }
  }

  get totalAmount(): number {
    return this.selectedSeats.length * this.ticketPrice;
  }

  isSelected(row: string, number: number): boolean {
    return this.selectedSeats.includes(row + number);
  }

  isBooked(row: string, number: number): boolean {
    return this.bookedSeats.includes(row + number);
  }
}
