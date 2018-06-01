import { Component, OnInit } from '@angular/core';
import { Http } from '@angular/http';

@Component({
  selector: 'app-values',
  templateUrl: './values.component.html',
  styleUrls: ['./values.component.css']
})
export class ValuesComponent implements OnInit {
  values: any;

  constructor(private http: Http) {}

  ngOnInit() {
    this.getValues();
  }

  getValues(): any {
    this.http.get('https://localhost:5001/api/values')
    .subscribe(response => {
      // console.log(response);
      this.values = response.json();
    });
  }
}
