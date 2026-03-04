òimport React from 'react';

export default function Tabs({ tabs, activeTab, onTabChange }) {
    return (
        <div className="tabs-container">
            {tabs.map((tab) => (
                <div
                    key={tab.id}
                    className={`tab ${activeTab === tab.id ? 'active' : ''}`}
                    onClick={() => onTabChange(tab.id)}
                >
                    {tab.label}
                </div>
            ))}
        </div>
    );
}
ò"(51ef8254d65def5408cd0e035a74a409c8f05b042Zfile:///c:/Users/Balqeyis/Desktop/KhosuRoom/KhosuRoom_Front/src/components/common/Tabs.jsx:;file:///c:/Users/Balqeyis/Desktop/KhosuRoom/KhosuRoom_Front