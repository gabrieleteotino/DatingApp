import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { ListsComponent } from './lists/lists.component';
import { MessagesComponent } from './messages/messages.component';
import { MemberListComponent } from './member-list/member-list.component';

export const ROUTES: Routes = [
    { path: 'home', component: HomeComponent },
    { path: 'members', component: MemberListComponent },
    { path: 'lists', component: ListsComponent },
    { path: 'messages', component: MessagesComponent },
    { path: '**', redirectTo: 'home', pathMatch: 'full' }
];
